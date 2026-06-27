using AutoMapper;
using GymMarket.API.DTOs.Trainer;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public override Task<IActionResult> GetAll()
        {
            return base.GetAll();
        }

        [HttpGet("{id}")]
        [Authorize]
        public override async Task<IActionResult> GetById(string id)
        {
            var trainer = await _repository.GetByIdAsync(id);
            if (trainer == null)
                return NotFound();

            if (CanManageTrainer(id))
                return Ok(trainer);

            return Ok(new
            {
                trainer.TrainerId,
                trainer.UserId,
                trainer.Name,
                trainer.Email,
                trainer.Certification,
                trainer.Category,
                trainer.Bio,
                trainer.Description,
                trainer.Experience,
                trainer.Rating,
                trainer.ProfilePicture,
                trainer.CreatedAt,
                trainer.UpdatedAt
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public override Task<IActionResult> Create([FromBody] TrainerCreateDTO createDto)
        {
            return base.Create(createDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Update(string id, [FromBody] TrainerUpdateDTO updateDto)
        {
            if (!CanManageTrainer(id))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                return NotFound();

            var existingUserId = existingEntity.UserId;

            _mapper.Map(updateDto, existingEntity);
            existingEntity.TrainerId = id;
            existingEntity.UserId = existingUserId;

            await _repository.Update(existingEntity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public override Task<IActionResult> Delete(string id)
        {
            return base.Delete(id);
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

        private bool CanManageTrainer(string trainerId)
        {
            return User.IsInRole(ApplicationRoles.Admin) ||
                   string.Equals(CurrentTrainerId(), trainerId, StringComparison.Ordinal);
        }

        private string? CurrentTrainerId()
        {
            return User.FindFirstValue("trainerId");
        }
    }
}
