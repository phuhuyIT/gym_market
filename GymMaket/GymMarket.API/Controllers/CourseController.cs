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
            var courses = await courseRepository.SearchAndFilterCoursesAsync(keyword, coachName, minPrice, maxPrice, minDuration, maxDuration, minRating, category);
            return Ok(courses);
        }
        protected override string GetEntityId(Course entity)
        {
            return entity.CourseId;
        }

        [HttpGet("get-courses-of-trainer/{trainerId}")]
        public async Task<IActionResult> GetCoursesOfTrainer(string trainerId)
        {
            var courses = await courseRepository.GetCoursesOfTrainer(trainerId);
            return Ok(courses);
        }
    }
}
