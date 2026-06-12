using GymMarket.API.DTOs.Payment;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Viewing a course's payments and approving/canceling them are trainer/admin actions.
    // Without the role restriction a student could call ok-payment to approve their own
    // pending payment and study without ever paying. On top of the role, every action is
    // scoped to courses the trainer owns (admins may touch any) — otherwise one trainer
    // could approve or cancel another trainer's payments.
    [Authorize(Roles = "Trainer,Admin")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICourseAccessService _courseAccessService;

        public PaymentsController(IPaymentRepository paymentRepository, ICourseAccessService courseAccessService)
        {
            _paymentRepository = paymentRepository;
            _courseAccessService = courseAccessService;
        }

        [HttpGet("get-payments-of-course/{courseId}")]
        public async Task<IActionResult> GetPaymentsOfCourse(string courseId)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var list = await _paymentRepository.GetPaymentsOfCourse(courseId);
            return Ok(list);
        }

        [HttpPost("ok-payment/{paymentId}")]
        public async Task<IActionResult> OkPayment(string paymentId)
        {
            if (!await CanManagePaymentAsync(paymentId))
                return Forbid();

            var payment = await _paymentRepository.OkPayment(paymentId);
            return Ok(payment);
        }

        [HttpPost("cancel-payment")]
        public async Task<IActionResult> CancelPayment(CancelPayment model)
        {
            if (!await CanManagePaymentAsync(model.PaymentId))
                return Forbid();

            var payment = await _paymentRepository.CancelPayment(model);
            return Ok(payment);
        }

        private async Task<bool> CanManagePaymentAsync(string paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment?.CourseId == null)
                return false;

            return await _courseAccessService.CanManageCourseAsync(User, payment.CourseId);
        }
    }
}
