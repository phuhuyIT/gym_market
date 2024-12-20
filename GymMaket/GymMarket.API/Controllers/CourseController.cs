using AutoMapper;
using GymMarket.API.DTOs.Course;
using GymMarket.API.Models;
using GymMarket.API.Repositories;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : GenericController<CourseCreateDTO, CourseUpdateDTO, Course, string>
    {
        private readonly ICourseRepository courseRepository;

        public CourseController(IGenericRepository<Course, string> repository,
            IMapper mapper, ICourseRepository courseRepository
            ) : base(repository, mapper)
        {
            this.courseRepository = courseRepository;
        }

        protected override string GetEntityId(Course entity)
        {
            return entity.CourseId;
        }

        [HttpPut("update-course")]
        public async Task<IActionResult> UpdateCourse([FromForm] CourseUpdateDTO courseUpdate)
        {
            var res = await courseRepository.UpdateCourse(courseUpdate);
            return StatusCode(res.StatusCode, new { res.Message, res.Errors });
        }

        [HttpGet("get-courses-of-trainer/{trainerId}")]
        public async Task<IActionResult> GetCoursesOfTrainer(string trainerId)
        {
            var courses = await courseRepository.GetCoursesOfTrainer(trainerId);
            return Ok(courses);
        }

        [HttpGet("get-course/{id}")]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var course = await courseRepository.GetCourse(id);

            if (course == null)
            {
                return BadRequest("không tìm thấy coourse");
            }
            return Ok(course);
        }

        [HttpGet("get-courses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await courseRepository.GetCourses();

            if (courses == null)
            {
                return BadRequest("không tìm thấy coourse");
            }
            return Ok(courses);
        }
    }
}

