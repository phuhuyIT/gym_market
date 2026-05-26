using AutoMapper;
using GymMarket.API.DTOs.Course;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : GenericController<CourseCreateDTO, CourseUpdateDTO, Course, string>
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(IGenericRepository<Course, string> repository,
            IMapper mapper, ICourseRepository courseRepository
            ) : base(repository, mapper)
        {
            _courseRepository = courseRepository;
        }

        protected override string GetEntityId(Course entity)
        {
            return entity.CourseId;
        }

        [AllowAnonymous]
        [HttpGet("search-and-filter")]
        public async Task<IActionResult> SearchAndFilterCourses(
            [FromQuery] string? keyword,
            [FromQuery] string? coachName,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? minDuration,
            [FromQuery] int? maxDuration,
            [FromQuery] double? minRating,
            [FromQuery] string? category)
        {
            var courses = await _courseRepository.SearchAndFilterCoursesAsync(keyword, coachName, minPrice, maxPrice, minDuration, maxDuration, minRating, category);
            return Ok(courses);
        }

        [HttpPut("update-course")]
        public async Task<IActionResult> UpdateCourse([FromForm] CourseUpdateDTO courseUpdate)
        {
            var res = await _courseRepository.UpdateCourse(courseUpdate);
            return StatusCode(res.StatusCode, new { res.Message, res.Errors });
        }

        [AllowAnonymous]
        [HttpGet("get-courses-of-trainer/{trainerId}")]
        public async Task<IActionResult> GetCoursesOfTrainer(string trainerId)
        {
            var courses = await _courseRepository.GetCoursesOfTrainer(trainerId);
            return Ok(courses);
        }

        [AllowAnonymous]
        [HttpGet("get-course/{id}")]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var course = await _courseRepository.GetCourse(id);

            if (course == null)
            {
                return BadRequest(new { errors = new string[] { "COURSE_NOT_FOUND" } });
            }
            return Ok(course);
        }

        [AllowAnonymous]
        [HttpGet("get-courses")]
        public async Task<IActionResult> GetCourses([FromQuery] string? category = null, [FromQuery] string? searchString = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 15)
        {
            var courses = await _courseRepository.GetCourses(pageIndex, pageSize, searchString, category);

            if (courses == null)
            {
                return BadRequest(new { errors = new string[] { "COURSES_NOT_FOUND" } });
            }
            return Ok(courses);
        }

    }
}
