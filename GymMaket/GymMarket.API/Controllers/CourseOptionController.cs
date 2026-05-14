using AutoMapper;
using GymMarket.API.DTOs.CourseOption;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseOptionController : GenericController<CourseOptionCreateDTO, CourseOptionUpdateDTO, CourseOption, string>
    {
        private readonly ICourseOptionRepository _courseOptionRepository;
        public CourseOptionController(ICourseOptionRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _courseOptionRepository = repository;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(string courseId)
        {
            var options = await _courseOptionRepository.GetByCourseIdAsync(courseId);
            return Ok(options);
        }

        protected override string GetEntityId(CourseOption entity)
        {
            return entity.OptionId;
        }
    }
}
