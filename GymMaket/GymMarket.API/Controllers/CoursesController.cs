using GymMarket.API.Models;
using GymMarket.API.Repositories;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRated(int topCount)
        {
            var topRated = await _courseRepository.GetTopRatedCoursesAsync(topCount);
            return Ok(topRated);
        }
        [HttpGet("newcourses")]
        public async Task<IActionResult> GetNewCourse()
        {
            var newCourse = await _courseRepository.GetNewestCoursesAsync(5);
            return Ok(newCourse);

        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest("Trainer data is null");
            }

            try
            {
                course.StartDate = DateTime.UtcNow;
                course.EndDate = DateTime.UtcNow;
                var createCourse = await _courseRepository.Add(course);

                return Ok(new { message = "Course created successfully.", courseId = createCourse.CourseId });
            }
            catch (DbUpdateException dbEx)
            {

                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return BadRequest($"Database update error: {innerMessage}");
            }
            catch (Exception ex)
            {
                // Bắt các lỗi khác
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            var trainers = await _courseRepository.Get(Id);
            if (trainers == null)
            {
                return BadRequest("Trainer data is null");
            }
            return Ok(trainers);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(string Id, [FromBody] Trainer trainer)
        {
            if (trainer == null)
            {
                return BadRequest();
            }
            var existingTrainer = await _courseRepository.Get(Id);
            if (existingTrainer == null)
            {
                return NotFound($"Trainer with Id {Id} not found");
            }
            try
            {

                await _courseRepository.Update(existingTrainer);
                return Ok("Trainer is Update Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string Id)
        {
            var existingTrainer = await _courseRepository.Get(Id);
            if (existingTrainer == null)
            {
                return NotFound($"Trainer with Id {Id} not found");
            }
            await _courseRepository.Delete(existingTrainer);
            return Ok("Đã Xóa Thành Công");
        }

    }
}
