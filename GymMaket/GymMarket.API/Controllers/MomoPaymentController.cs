using GymMarket.API.DTOs.Momo;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            var response = await _momoService.CreatePaymentAsync(dto);
            if (response == null)
            {
                return NotFound(new { error = "COURSE_NOT_FOUND" });
            }
            return Ok(new { payUrl = response.PayUrl });
        }

        [AllowAnonymous]
        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack([FromQuery] MomoCallbackDto callback)
        {
            if (!_momoService.VerifySignature(callback))
                return BadRequest(new { error = "INVALID_SIGNATURE" });

            var redirectUrl = _configuration["MomoAPI:PaymentRedirectUrl"] ?? "/client/course-registration";

            if (callback.ResultCode == 0)
            {
                var extraDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(callback.ExtraData));
                var extraData = JsonConvert.DeserializeObject<dynamic>(extraDataJson);

                string studentId = extraData!.StudentId;
                string courseId = extraData!.CourseId;

                await _paymentRepository.CreatePayment(new Payment
                {
                    PaymentId = callback.OrderId,
                    CourseId = courseId,
                    StudentId = studentId,
                    PaymentAmount = decimal.Parse(callback.Amount),
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = "COMPLETED",
                    PaymentType = "MOMO",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return Redirect(redirectUrl);
        }

        [AllowAnonymous]
        [HttpPost("MomoNotify")]
        public async Task<IActionResult> MomoNotify([FromBody] MomoCallbackDto callback)
        {
            if (!_momoService.VerifySignature(callback))
                return BadRequest(new { error = "INVALID_SIGNATURE" });

            return Ok();
        }
    }

    public class MomoCallbackDto
    {
        public string PartnerCode { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public string RequestId { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public string OrderInfo { get; set; } = null!;
        public string OrderType { get; set; } = null!;
        public string TransId { get; set; } = null!;
        public int ResultCode { get; set; }
        public string Message { get; set; } = null!;
        public string PayType { get; set; } = null!;
        public string ResponseTime { get; set; } = null!;
        public string ExtraData { get; set; } = null!;
        public string Signature { get; set; } = null!;
    }
}
