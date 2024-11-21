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
        public CourseOptionController(IGenericRepository<CourseOption, string> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override string GetEntityId(CourseOption entity)
        {
            return entity.OptionId;
        }
    }
}
