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

            if (result != null)
            {
                return Ok(new { Message = "COURSE_REGISTRATION_SUCCESS", Registration = result });
            }
            return BadRequest(new { Message = "COURSE_REGISTRATION_FAILED" });
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

        // Registrations belong to the authenticated student only — never trust a
        // client-sent id. The claim is an empty string for non-student users.
        private string CurrentStudentId()
        {
            return User.FindFirstValue("studentId") ?? string.Empty;
        }
    }
}
