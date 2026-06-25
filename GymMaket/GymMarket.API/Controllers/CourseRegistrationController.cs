using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseRegistrationController : ControllerBase
    {
        private readonly ICourseRegistrationRepository _courseRegistrationRepository;

        public CourseRegistrationController(ICourseRegistrationRepository courseRegistrationRepository)
        {
            _courseRegistrationRepository = courseRegistrationRepository;
        }


        [HttpPost("register-course")]
        public async Task<IActionResult> RegisterCourse(RegisterCourseDto dto)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
            }

            // Call registration function in repository
            var result = await _courseRegistrationRepository.RegisterCourseAsync(dto, studentId);

            if (result.Success && result.Registration != null)
            {
                return Ok(new { Message = "COURSE_REGISTRATION_SUCCESS", Registration = result.Registration });
            }

            return result.ErrorCode switch
            {
                CourseRegistrationErrorCode.CourseNotFound => NotFound(new { Message = result.ErrorCode }),
                CourseRegistrationErrorCode.CourseNotPublished => Conflict(new { Message = result.ErrorCode }),
                CourseRegistrationErrorCode.CourseFull => Conflict(new { Message = result.ErrorCode }),
                CourseRegistrationErrorCode.InvalidCourseOption => Conflict(new { Message = result.ErrorCode }),
                _ => BadRequest(new { Message = result.ErrorCode ?? "COURSE_REGISTRATION_FAILED" })
            };
        }


        [HttpPost("cancel-registration/{registrationId}")]
        public async Task<IActionResult> CancelRegistration(string registrationId)
        {
            var isCancelled = await _courseRegistrationRepository.CancelRegistrationAsync(registrationId, CurrentStudentId());
            if (isCancelled)
            {
                return Ok(new { Message = "REGISTRATION_CANCELLED_SUCCESS" });
            }
            return BadRequest(new { Message = "REGISTRATION_CANCEL_FAILED" });
        }

        [HttpPost("initialize-payment/{registrationId}")]
        public async Task<IActionResult> InitializePayment(string registrationId, [FromQuery] decimal initialPayment)
        {
            var isInitialized = await _courseRegistrationRepository.InitializePaymentAsync(registrationId, initialPayment, CurrentStudentId());
            if (isInitialized)
            {
                return Ok(new { Message = "PAYMENT_INITIALIZATION_SUCCESS" });
            }
            return BadRequest(new { Message = "PAYMENT_INITIALIZATION_FAILED" });
        }

        [HttpGet("get-course-registrations")]
        public async Task<IActionResult> GetCourseRegistrations()
        {
            var list = await _courseRegistrationRepository.GetCourseRegistrations(CurrentStudentId());
            return Ok(list);
        }

        // Bank-transfer payment details for a course the caller has registered for.
        [HttpGet("payment-info/{courseId}")]
        public async Task<IActionResult> GetPaymentInfo(string courseId)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
            }

            var info = await _courseRegistrationRepository.GetCoursePaymentInfo(studentId, courseId);
            if (info == null)
            {
                return NotFound(new { Message = "REGISTRATION_NOT_FOUND" });
            }
            return Ok(info);
        }

        // The student taps "I've paid" on the payment screen. This notifies the trainer
        // to verify the bank transfer; it does NOT mark the payment paid (the trainer
        // approves it). Returns the current payment info so the client can react if the
        // trainer has already confirmed.
        [HttpPost("confirm-payment/{courseId}")]
        public async Task<IActionResult> ConfirmPayment(string courseId)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
            }

            var info = await _courseRegistrationRepository.ConfirmPaymentByStudent(studentId, courseId);
            if (info == null)
            {
                return NotFound(new { Message = "REGISTRATION_NOT_FOUND" });
            }
            return Ok(info);
        }

        // Registrations belong to the authenticated student only — never trust a
        // client-sent id. The claim is an empty string for non-student users.
        private string CurrentStudentId()
        {
            return User.FindFirstValue("studentId") ?? string.Empty;
        }
    }
}
