using AutoMapper;
using GymMarket.API.DTOs.LectureMaterial;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureMaterialController : GenericController<LectureMaterialCreateDTO, LectureMaterialUpdateDTO, LectureMaterial, string>
    {
        private readonly ILectureMaterialRepository _lectureMaterialRepository;
        private readonly ICourseAccessService _courseAccessService;

        public LectureMaterialController(ILectureMaterialRepository repository, IMapper mapper, ICourseAccessService courseAccessService) : base(repository, mapper)
        {
            _lectureMaterialRepository = repository;
            _courseAccessService = courseAccessService;
        }

        protected override string GetEntityId(LectureMaterial entity)
        {
            return entity.MaterialId;
        }

        [HttpGet("lecture/{lectureId}")]
        public async Task<IActionResult> GetMaterialsByLectureId(string lectureId)
        {
            // Students may only study materials of an unlocked lesson; trainers, only their own.
            var unlockState = await _courseAccessService.GetLectureUnlockStateAsync(User, lectureId);
            if (unlockState.IsLocked)
                return Forbid();

            var materials = await _lectureMaterialRepository.GetMaterialsByLectureIdAsync(lectureId);
            var materialDtos = _mapper.Map<IEnumerable<GetLectureMaterialDto>>(materials);
            return Ok(materialDtos);
        }

        // Bulk listing of every material is a staff-only operation.
        [HttpGet]
        [Authorize(Roles = "Trainer,Admin")]
        public override Task<IActionResult> GetAll() => base.GetAll();

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(string id)
        {
            if (!await _courseAccessService.CanAccessMaterialAsync(User, id))
                return Forbid();

            return await base.GetById(id);
        }

        [HttpPost]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Create([FromBody] LectureMaterialCreateDTO createDto)
        {
            if (string.IsNullOrEmpty(createDto.LectureId))
                return BadRequest(new { error = "LectureId is required." });

            if (!await _courseAccessService.CanManageLectureAsync(User, createDto.LectureId))
                return Forbid();

            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Update(string id, [FromBody] LectureMaterialUpdateDTO updateDto)
        {
            if (!await _courseAccessService.CanManageMaterialAsync(User, id))
                return Forbid();

            return await base.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Delete(string id)
        {
            if (!await _courseAccessService.CanManageMaterialAsync(User, id))
                return Forbid();

            return await base.Delete(id);
        }
    }
}
