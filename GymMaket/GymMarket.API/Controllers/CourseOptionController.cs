using AutoMapper;
using GymMarket.API.DTOs.CourseOption;
using GymMarket.API.Models;
using GymMarket.API.Repositories;
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

        public CourseOptionController(IGenericRepository<CourseOption, string> repository, IMapper mapper, ICourseOptionRepository courseOptionRepository) : base(repository, mapper)
        {
            this._courseOptionRepository = courseOptionRepository;
        }

        protected override string GetEntityId(CourseOption entity)
        {
            return entity.OptionId;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOption()
        {
            var courseOptions = await _courseOptionRepository.GetAllAsync();
            return Ok(courseOptions);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>GetById(string courseId)
        {
            var course = await _courseOptionRepository.GetByIdAsync(courseId);
            return Ok(course);
        }
       
        

        
    }
}
