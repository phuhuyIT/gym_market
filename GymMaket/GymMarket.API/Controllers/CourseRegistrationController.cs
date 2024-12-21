
using AutoMapper;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using GymMarket.API.Repositories;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRegistrationController : GenericController<CourseRegistrationCreateDto, CourseRegistrationUpdateDto,CourseRegistration, string>
    {
        public CourseRegistrationController(IGenericRepository<CourseRegistration, string> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override string GetEntityId(CourseRegistration entity)
        {
            return entity.RegistrationId;
        }
    }
}
