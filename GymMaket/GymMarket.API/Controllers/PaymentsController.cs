using GymMarket.API.DTOs.Payment;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Viewing a course's payments and approving/canceling them are trainer/admin actions.
    // Without the role restriction a student could call ok-payment to approve their own
    // pending payment and study without ever paying.
    [Authorize(Roles = "Trainer,Admin")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpGet("get-payments-of-course/{courseId}")]
        public async Task<IActionResult> GetPaymentsOfCourse(string courseId)
        {
            var list = await _paymentRepository.GetPaymentsOfCourse(courseId);
            return Ok(list);
        }

        [HttpPost("ok-payment/{paymentId}")]
        public async Task<IActionResult> OkPayment(string paymentId)
        {
            var payment = await _paymentRepository.OkPayment(paymentId);
            return Ok(payment);
        }

        [HttpPost("cancel-payment")]
        public async Task<IActionResult> CancelPayment(CancelPayment model)
        {
            var payment = await _paymentRepository.CancelPayment(model);
            return Ok(payment);
        }
    }
}
