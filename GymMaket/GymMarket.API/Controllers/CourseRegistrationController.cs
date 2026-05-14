using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            // Call registration function in repository
            var result = await _courseRegistrationRepository.RegisterCourseAsync(dto);

            if (result != null)
            {
                return Ok(new { Message = "COURSE_REGISTRATION_SUCCESS", Registration = result });
            }
            return BadRequest(new { Message = "COURSE_REGISTRATION_FAILED" });
        }


        [HttpPost("cancel-registration/{registrationId}")]
        public async Task<IActionResult> CancelRegistration(string registrationId)
        {
            var isCancelled = await _courseRegistrationRepository.CancelRegistrationAsync(registrationId);
            if (isCancelled)
            {
                return Ok(new { Message = "REGISTRATION_CANCELLED_SUCCESS" });
            }
            return BadRequest(new { Message = "REGISTRATION_CANCEL_FAILED" });
        }

        [HttpPost("initialize-payment/{registrationId}")]
        public async Task<IActionResult> InitializePayment(string registrationId, [FromQuery] decimal initialPayment)
        {
            var isInitialized = await _courseRegistrationRepository.InitializePaymentAsync(registrationId, initialPayment);
            if (isInitialized)
            {
                return Ok(new { Message = "PAYMENT_INITIALIZATION_SUCCESS" });
            }
            return BadRequest(new { Message = "PAYMENT_INITIALIZATION_FAILED" });
        }

        [HttpGet("get-course-registrations/{studentId}")]
        public async Task<IActionResult> GetCourseRegistrations(string studentId)
        {
            var list = await _courseRegistrationRepository.GetCourseRegistrations(studentId);
            return Ok(list);
        }
    }
}