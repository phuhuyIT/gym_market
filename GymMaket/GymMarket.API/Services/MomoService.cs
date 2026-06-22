using GymMarket.API.Data;
using GymMarket.API.DTOs.Momo;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Services
{
    public class MomoService
    {
        private readonly IOptions<Models.MomoOptionModel> _options;
        private readonly GymMarketContext _context;

        public MomoService(IOptions<Models.MomoOptionModel> options, GymMarketContext context)
        {
            _options = options;
            _context = context;
        }

        public async Task<MomoCreatePaymentResponseModel?> CreatePaymentAsync(string courseId, string studentId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null || course.Status != CourseStatus.Published) return null;

            var registration = await _context.CourseRegistrations
                .FirstOrDefaultAsync(cr => cr.CourseId == courseId && cr.StudentId == studentId);
            if (registration == null
                || registration.PaymentStatus == PaymentStatus.Canceled
                || registration.PaymentStatus == PaymentStatus.Expired)
            {
                return null;
            }

            if (await IsCourseFullForNewStudentAsync(courseId, studentId, course.MaxParticipants))
            {
                return null;
            }

            string paymentId = Guid.NewGuid().ToString("N");
            double paymentAmount = (double)((course.Price ?? 0) + (course.AdditionalPrice ?? 0));

            // Store studentId and courseId in extraData as JSON so the signed
            // callback/IPN can tell which student paid for which course.
            var extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { StudentId = studentId, CourseId = courseId })));

            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={paymentId}" +
                $"&amount={paymentAmount}" +
                $"&orderId={paymentId}" +
                $"&returnUrl={_options.Value.ReturnUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&extraData={extraData}";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = paymentId,
                amount = paymentAmount.ToString(),
                requestId = paymentId,
                extraData = extraData,
                signature = signature,
                orderInfo = $"Payment for course: {course.Title}"
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content!);
            return momoResponse!;
        }

        private async Task<bool> IsCourseFullForNewStudentAsync(string courseId, string studentId, int? maxParticipants)
        {
            if (maxParticipants == null || maxParticipants <= 0)
                return false;

            var studentAlreadyActive = await _context.CourseRegistrations.AnyAsync(cr =>
                cr.CourseId == courseId
                && cr.StudentId == studentId
                && cr.PaymentStatus != PaymentStatus.Canceled
                && cr.PaymentStatus != PaymentStatus.Expired);
            if (studentAlreadyActive)
                return false;

            var activeRegistrations = await _context.CourseRegistrations
                .Where(cr => cr.CourseId == courseId
                    && cr.PaymentStatus != PaymentStatus.Canceled
                    && cr.PaymentStatus != PaymentStatus.Expired)
                .Select(cr => cr.StudentId)
                .Distinct()
                .CountAsync();

            return activeRegistrations >= maxParticipants;
        }

        public bool VerifySignature(MomoCallbackDto callback)
        {
            var rawData =
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={callback.Amount}" +
                $"&extraData={callback.ExtraData}" +
                $"&message={callback.Message}" +
                $"&orderId={callback.OrderId}" +
                $"&orderInfo={callback.OrderInfo}" +
                $"&orderType={callback.OrderType}" +
                $"&partnerCode={callback.PartnerCode}" +
                $"&payType={callback.PayType}" +
                $"&requestId={callback.RequestId}" +
                $"&responseTime={callback.ResponseTime}" +
                $"&resultCode={callback.ResultCode}" +
                $"&transId={callback.TransId}";

            var expectedSignature = ComputeHmacSha256(rawData, _options.Value.SecretKey);
            return string.Equals(expectedSignature, callback.Signature, StringComparison.OrdinalIgnoreCase);
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;

            return new MomoExecuteResponseModel()
            {
                Amount = amount!,
                OrderId = orderId!,
                OrderInfo = orderInfo!
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}
