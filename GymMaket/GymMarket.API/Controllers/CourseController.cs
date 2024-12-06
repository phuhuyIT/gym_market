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

        [HttpGet("get-courses-of-trainer/{trainerId}")]
        public async Task<IActionResult> GetCoursesOfTrainer(string trainerId)
        {
            var courses = await courseRepository.GetCoursesOfTrainer(trainerId);
            return Ok(courses);
        }
    }
}
