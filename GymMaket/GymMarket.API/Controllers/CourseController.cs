using AutoMapper;
using GymMarket.API.DTOs.Course;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : GenericController<CourseCreateDTO, CourseUpdateDTO, Course, string>
    {
        private readonly ICourseRepository _courseRepository;
        public CourseController(IGenericRepository<Course, string> repository, IMapper mapper) : base(repository, mapper)
        {
            _courseRepository = repository as ICourseRepository ?? throw new ArgumentNullException(nameof(repository), "Repository must be of type ICourseRepository.");
        }

        
        protected override string GetEntityId(Course entity)
        {
            return entity.CourseId;
        }
        [HttpGet("trainer/{trainerId}")]
        public async Task<IActionResult> GetCoursesByTrainer(string trainerId)
        {
            if (string.IsNullOrWhiteSpace(trainerId))
                return BadRequest("Trainer ID cannot be null or empty.");

            var courses = await _courseRepository.GetCoursesByTrainer(trainerId);

            if (courses == null || !courses.Any())
                return NotFound($"No courses found for trainer with ID {trainerId}.");

            return Ok(courses);
        }
    }
}
