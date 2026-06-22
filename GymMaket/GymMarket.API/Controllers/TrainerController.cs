using AutoMapper;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : GenericController<TrainerCreateDTO, TrainerUpdateDTO, Trainer, string>
    {
        private readonly ITrainerRepository _trainerRepository;

        public TrainerController(ITrainerRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _trainerRepository = repository;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = Defaults.PageSize,
            [FromQuery] string? category = null,
            [FromQuery] bool? eliteOnly = null,
            [FromQuery] decimal? minRating = null,
            [FromQuery] decimal? maxRating = null,
            [FromQuery] int? minExperience = null,
            [FromQuery] int? maxExperience = null,
            [FromQuery] string? status = null)
        {
            var result = await _trainerRepository.SearchTrainers(
                pageIndex,
                pageSize,
                search,
                category,
                eliteOnly,
                minRating,
                maxRating,
                minExperience,
                maxExperience,
                status);
            return Ok(result);
        }

        protected override string GetEntityId(Trainer entity)
        {
            return entity.TrainerId;
        }
    }
}
