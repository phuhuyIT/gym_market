using AutoMapper;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRatingController : GenericController<CourseRating, string>
    {
        public CourseRatingController(IGenericRepository<CourseRating, string> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override string GetEntityId(CourseRating entity)
        {
            return entity.RatingId;
        }
    }
}
