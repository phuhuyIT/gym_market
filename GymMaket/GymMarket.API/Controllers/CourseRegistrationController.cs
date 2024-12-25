using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using GymMarket.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRegistrationController : ControllerBase
    {
        private readonly CourseRegistrationRepository _courseRegistrationRepository;

        public CourseRegistrationController(CourseRegistrationRepository courseRegistrationRepository)
        {
            _courseRegistrationRepository = courseRegistrationRepository;
        }


        [HttpPost("register-course")]
        public async Task<IActionResult> RegisterCourse(RegisterCourseDto dto)
        {
            // Gọi hàm đăng ký trong repository
            var result = await _courseRegistrationRepository.RegisterCourseAsync(dto);

            if (result != null)
            {
                return Ok(new { Message = "Đăng ký khóa học thành công!", Registration = result });
            }
            return BadRequest(new { Message = "Đăng ký khóa học thất bại. Vui lòng kiểm tra lại thông tin." });
        }


        [HttpPost("cancel-registration/{registrationId}")]
        public async Task<IActionResult> CancelRegistration(string registrationId)
        {
            var isCancelled = await _courseRegistrationRepository.CancelRegistrationAsync(registrationId);
            if (isCancelled)
            {
                return Ok(new { Message = "Hủy đăng ký thành công!" });
            }
            return BadRequest(new { Message = "Hủy đăng ký thất bại. Vui lòng kiểm tra lại thông tin." });
        }

        [HttpPost("initialize-payment/{registrationId}")]
        public async Task<IActionResult> InitializePayment(string registrationId, [FromQuery] decimal initialPayment)
        {
            var isInitialized = await _courseRegistrationRepository.InitializePaymentAsync(registrationId, initialPayment);
            if (isInitialized)
            {
                return Ok(new { Message = "Khởi tạo thanh toán thành công!" });
            }
            return BadRequest(new { Message = "Khởi tạo thanh toán thất bại. Vui lòng thử lại." });
        }

        [HttpGet("get-course-registrations/{studentId}")]
        public async Task<IActionResult> GetCourseRegistrations(string studentId)
        {
            var list = await _courseRegistrationRepository.GetCourseRegistrations(studentId);
            return Ok(list);
        }
    }
}