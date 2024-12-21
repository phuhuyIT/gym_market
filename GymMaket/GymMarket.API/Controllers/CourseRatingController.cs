using AutoMapper;
using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.Models;
using GymMarket.API.Repositories;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRatingController: GenericController<CourseRatingCreateDto, CourseRatingUpdateDto, CourseRating, string>
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