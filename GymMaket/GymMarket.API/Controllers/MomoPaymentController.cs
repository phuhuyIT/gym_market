using GymMarket.API.DTOs.Momo;
using GymMarket.API.DTOs.Payment;
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
        private readonly IConfiguration _configuration;

        public MomoPaymentController(MomoService momoService, IPaymentRepository paymentRepository, IConfiguration configuration)
        {
            _momoService = momoService;
            _paymentRepository = paymentRepository;
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

            var response = await _momoService.CreatePaymentAsync(dto.CourseId, studentId);
            if (response == null)
            {
                return NotFound(new { error = "COURSE_NOT_FOUND" });
            }
            return Ok(new { payUrl = response.PayUrl });
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

            await TryConfirmPaymentAsync(callback);

            var redirectUrl = _configuration["MomoAPI:PaymentRedirectUrl"] ?? "/client/course-registration";
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

            await TryConfirmPaymentAsync(callback);
            return Ok();
        }

        private async Task TryConfirmPaymentAsync(MomoCallbackDto callback)
        {
            if (callback.ResultCode != Defaults.MomoSuccessResultCode || string.IsNullOrEmpty(callback.ExtraData))
                return;

            var extraDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(callback.ExtraData));
            var extraData = JsonConvert.DeserializeObject<MomoExtraData>(extraDataJson);

            if (extraData == null || string.IsNullOrEmpty(extraData.StudentId) || string.IsNullOrEmpty(extraData.CourseId))
                return;

            await _paymentRepository.ConfirmCoursePayment(extraData.StudentId, extraData.CourseId, PaymentType.Momo);
        }
    }
}
