using GymMarket.API.DTOs.Payment;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpGet("get-payments-ofcourse/{courseId}")]
        public async Task<IActionResult> GetPaymentsOfCourse(string courseId)
        {
            var list = await _paymentRepository.GetPaymentsOfCourse(courseId);
            return Ok(list);
        }

        [HttpPost("ok-payment/{paymentId}")]
        public async Task<IActionResult> OkePayment(string paymentId)
        {
            var r = await _paymentRepository.OkPayment(paymentId);
            return Ok(r);
        }

        [HttpPost("cancel-payment")]
        public async Task<IActionResult> CancelPayment(CancelPayment paymentId)
        {
            var r = await _paymentRepository.CancelPayment(paymentId);
            return Ok(r);
        }
    }
}
