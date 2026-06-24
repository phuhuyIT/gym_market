using AutoMapper;
using GymMarket.API.DTOs.CourseOption;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseOptionController : GenericController<CourseOptionCreateDTO, CourseOptionUpdateDTO, CourseOption, string>
    {
        private readonly ICourseOptionRepository _courseOptionRepository;
        private readonly ICourseAccessService _courseAccessService;

        public CourseOptionController(
            ICourseOptionRepository repository,
            IMapper mapper,
            ICourseAccessService courseAccessService) : base(repository, mapper)
        {
            _courseOptionRepository = repository;
            _courseAccessService = courseAccessService;
        }

        [HttpGet]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> GetAll()
        {
            if (User.IsInRole(ApplicationRoles.Admin))
            {
                return await base.GetAll();
            }

            var trainerId = User.FindFirstValue("trainerId");
            if (string.IsNullOrWhiteSpace(trainerId))
            {
                return Forbid();
            }

            var options = await _courseOptionRepository.GetByTrainerIdAsync(trainerId);
            return Ok(options);
        }

        [AllowAnonymous]
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(string courseId)
        {
            var options = await _courseOptionRepository.GetByCourseIdAsync(courseId);
            return Ok(options);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> GetById(string id)
        {
            var existing = await _courseOptionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            if (!await _courseAccessService.CanManageCourseAsync(User, existing.CourseId ?? string.Empty))
                return Forbid();

            return Ok(existing);
        }

        [HttpPost]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Create([FromBody] CourseOptionCreateDTO createDto)
        {
            if (string.IsNullOrEmpty(createDto.CourseId))
                return BadRequest(new { errors = new[] { "COURSE_ID_REQUIRED" } });

            if (!await _courseAccessService.CanManageCourseAsync(User, createDto.CourseId))
                return Forbid();

            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Update(string id, [FromBody] CourseOptionUpdateDTO updateDto)
        {
            var existing = await _courseOptionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            if (!await _courseAccessService.CanManageCourseAsync(User, existing.CourseId ?? string.Empty))
                return Forbid();

            if (!string.Equals(existing.CourseId, updateDto.CourseId, StringComparison.Ordinal)
                && !await _courseAccessService.CanManageCourseAsync(User, updateDto.CourseId))
            {
                return Forbid();
            }

            return await base.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Delete(string id)
        {
            var existing = await _courseOptionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            if (!await _courseAccessService.CanManageCourseAsync(User, existing.CourseId ?? string.Empty))
                return Forbid();

            return await base.Delete(id);
        }

        protected override string GetEntityId(CourseOption entity)
        {
            return entity.OptionId;
        }
    }
}
