using GymMarket.API.DTOs.Momo;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;
using GymMarket.API.Data;
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
        private readonly IOptions<MomoOptionModel> _options;
        private readonly GymMarketContext _context;

        public MomoService(IOptions<MomoOptionModel> options, GymMarketContext context)
        {
            _options = options;
            _context = context;
        }

        public async Task<MomoCreatePaymentResponseModel?> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null) return null;

            string paymentId = Guid.NewGuid().ToString("N");
            double paymentAmount = (double)((course.Price ?? 0) + (course.AdditionalPrice ?? 0));

            // Store studentId and courseId in extraData as JSON
            var extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { dto.StudentId, dto.CourseId })));

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

        public bool VerifySignature(Controllers.MomoCallbackDto callback)
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
