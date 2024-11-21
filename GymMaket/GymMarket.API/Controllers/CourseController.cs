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
        public CourseController(IGenericRepository<Course, string> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        
        protected override string GetEntityId(Course entity)
        {
            return entity.CourseId;
        }
    }
}
