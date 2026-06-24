using GymMarket.API.Data;
using GymMarket.API.DTOs.Momo;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MomoPaymentController : ControllerBase
    {
        private readonly MomoService _momoService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentEventService _paymentEventService;
        private readonly GymMarketContext _context;
        private readonly IConfiguration _configuration;

        public MomoPaymentController(
            MomoService momoService,
            IPaymentRepository paymentRepository,
            IPaymentEventService paymentEventService,
            GymMarketContext context,
            IConfiguration configuration)
        {
            _momoService = momoService;
            _paymentRepository = paymentRepository;
            _paymentEventService = paymentEventService;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] CreatePaymentDto dto)
        {
            // The payment is always made by the authenticated student — never trust
            // a client-sent id, or anyone could mint payment URLs for other students.
            var studentId = User.FindFirstValue("studentId");
            if (string.IsNullOrEmpty(studentId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "NOT_A_STUDENT" });
            }

            var result = await _momoService.CreatePaymentAsync(dto.CourseId, studentId);
            if (!result.Success || result.Payment == null)
            {
                return result.ErrorCode switch
                {
                    CourseRegistrationErrorCode.CourseNotFound => NotFound(new { error = result.ErrorCode }),
                    CourseRegistrationErrorCode.RegistrationNotFound => NotFound(new { error = result.ErrorCode }),
                    CourseRegistrationErrorCode.CourseNotPublished => Conflict(new { error = result.ErrorCode }),
                    CourseRegistrationErrorCode.CourseFull => Conflict(new { error = result.ErrorCode }),
                    CourseRegistrationErrorCode.RegistrationCanceled => Conflict(new { error = result.ErrorCode }),
                    CourseRegistrationErrorCode.MomoNotConfigured => StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = result.ErrorCode }),
                    CourseRegistrationErrorCode.MomoProviderUnavailable => StatusCode(StatusCodes.Status502BadGateway, new { error = result.ErrorCode }),
                    _ => BadRequest(new { error = result.ErrorCode ?? "MOMO_PAYMENT_FAILED" })
                };
            }
            return Ok(new { payUrl = result.Payment.PayUrl });
        }

        // Browser redirect (returnUrl). This is user-controllable and not guaranteed to fire,
        // so it only acts as a best-effort fallback to the IPN below — both go through the
        // same idempotent confirm path, so a double-fire never double-records.
        [AllowAnonymous]
        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack([FromQuery] MomoCallbackDto callback)
        {
            if (!_momoService.VerifySignature(callback))
                return BadRequest(new { error = "INVALID_SIGNATURE" });

            var result = await HandleGatewayCallbackAsync(callback, PaymentEventType.MomoReturned);

            var redirectUrl = BuildPaymentRedirectUrl(result, callback);
            return Redirect(redirectUrl);
        }

        // Server-to-server IPN. This is the authoritative source of truth for a successful
        // payment because, unlike the browser redirect, it fires even if the user closes the tab.
        [AllowAnonymous]
        [HttpPost("MomoNotify")]
        public async Task<IActionResult> MomoNotify([FromBody] MomoCallbackDto callback)
        {
            if (!_momoService.VerifySignature(callback))
                return BadRequest(new { error = "INVALID_SIGNATURE" });

            var result = await HandleGatewayCallbackAsync(callback, PaymentEventType.MomoIpnReceived);
            return Ok(new { message = result.Status, courseId = result.CourseId });
        }

        private async Task<MomoGatewayResult> HandleGatewayCallbackAsync(MomoCallbackDto callback, string receiptEventType)
        {
            var extraData = TryReadExtraData(callback.ExtraData);
            var result = new MomoGatewayResult
            {
                CourseId = extraData?.CourseId,
                StudentId = extraData?.StudentId,
                ResultCode = callback.ResultCode,
                Status = callback.ResultCode == Defaults.MomoSuccessResultCode ? "success" : "canceled",
            };

            if (string.IsNullOrWhiteSpace(callback.OrderId))
            {
                result.Status = "failed";
                return result;
            }

            if (callback.ResultCode == Defaults.MomoSuccessResultCode)
            {
                var payment = await _paymentRepository.ConfirmGatewayPayment(callback.OrderId, PaymentType.Momo);
                result.CourseId ??= payment?.CourseId;
                result.StudentId ??= payment?.StudentId;
                result.Status = payment == null ? "failed" : "success";
                await RecordMomoReceiptEventAsync(payment, callback, receiptEventType);
                return result;
            }

            var note = string.IsNullOrWhiteSpace(callback.Message)
                ? $"Momo payment failed with result code {callback.ResultCode}."
                : callback.Message;
            var canceled = await _paymentRepository.CancelGatewayPayment(callback.OrderId, note);
            result.CourseId ??= canceled?.CourseId;
            result.StudentId ??= canceled?.StudentId;
            result.Status = canceled == null ? "failed" : "canceled";
            await RecordMomoReceiptEventAsync(canceled, callback, receiptEventType);
            return result;
        }

        private async Task RecordMomoReceiptEventAsync(Payment? payment, MomoCallbackDto callback, string eventType)
        {
            if (payment == null)
                return;

            await _paymentEventService.AddPaymentEventAsync(
                payment.PaymentId,
                eventType,
                payment.PaymentStatus,
                payment.PaymentStatus,
                PaymentEventSource.Momo,
                callback.Message,
                JsonConvert.SerializeObject(callback));
            await _context.SaveChangesAsync();
        }

        private MomoExtraData? TryReadExtraData(string? extraData)
        {
            if (string.IsNullOrWhiteSpace(extraData))
                return null;

            try
            {
                var extraDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(extraData));
                return JsonConvert.DeserializeObject<MomoExtraData>(extraDataJson);
            }
            catch
            {
                return null;
            }
        }

        private string BuildPaymentRedirectUrl(MomoGatewayResult result, MomoCallbackDto callback)
        {
            var configuredUrl = _configuration["MomoAPI:PaymentRedirectUrl"] ?? "/client/course-registration";
            var courseId = result.CourseId;
            var targetUrl = BuildCoursePaymentUrl(configuredUrl, courseId);

            var separator = targetUrl.Contains('?') ? "&" : "?";
            var message = Uri.EscapeDataString(callback.Message ?? string.Empty);
            return $"{targetUrl}{separator}momoResult={result.Status}&momoCode={callback.ResultCode}&momoMessage={message}";
        }

        private string BuildCoursePaymentUrl(string configuredUrl, string? courseId)
        {
            if (string.IsNullOrWhiteSpace(courseId))
                return configuredUrl;

            var paymentPath = $"/client/course-payment/{Uri.EscapeDataString(courseId)}";
            if (Uri.TryCreate(configuredUrl, UriKind.Absolute, out var absoluteUri))
            {
                return $"{absoluteUri.Scheme}://{absoluteUri.Authority}{paymentPath}";
            }

            return paymentPath;
        }

        private class MomoGatewayResult
        {
            public string Status { get; set; } = "failed";
            public int ResultCode { get; set; }
            public string? CourseId { get; set; }
            public string? StudentId { get; set; }
        }
    }
}
