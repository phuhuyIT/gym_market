using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Lecture;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : GenericController<LectureCreateDTO, LectureUpdateDTO, Lecture, string>
    {
        private readonly ILectureRepository _lectureRepository;
        private readonly ICourseAccessService _courseAccessService;
        private readonly GymMarketContext _context;

        public LectureController(
            ILectureRepository repository,
            IMapper mapper,
            ICourseAccessService courseAccessService,
            GymMarketContext context) : base(repository, mapper)
        {
            _lectureRepository = repository;
            _courseAccessService = courseAccessService;
            _context = context;
        }

        protected override string GetEntityId(Lecture entity)
        {
            return entity.LectureId;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLecturesByCourseId(string courseId)
        {
            // Students may only study a course they have paid for; trainers, only their own.
            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            var lectures = await _lectureRepository.GetLecturesByCourseIdAsync(courseId);
            if (User.IsInRole(ApplicationRoles.Student))
            {
                lectures = lectures.Where(l => l.IsPublished && l.Module?.IsPublished != false);
            }

            var lectureDtos = new List<GetLectureDto>();
            foreach (var lecture in lectures)
            {
                var dto = _mapper.Map<GetLectureDto>(lecture);
                var unlockState = await _courseAccessService.GetLectureUnlockStateAsync(User, lecture.LectureId);
                dto.IsLocked = unlockState.IsLocked;
                dto.LockReason = unlockState.Reason;
                dto.UnlocksAt = unlockState.UnlocksAt;
                lectureDtos.Add(dto);
            }
            return Ok(lectureDtos);
        }

        // Bulk listing of every lecture is a staff-only operation.
        [HttpGet]
        [Authorize(Roles = "Trainer,Admin")]
        public override Task<IActionResult> GetAll() => base.GetAll();

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(string id)
        {
            if (!await _courseAccessService.CanAccessLectureAsync(User, id))
                return Forbid();

            return await base.GetById(id);
        }

        [HttpPost]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Create([FromBody] LectureCreateDTO createDto)
        {
            if (string.IsNullOrEmpty(createDto.CourseId))
                return BadRequest(new { error = "CourseId is required." });

            if (!await _courseAccessService.CanManageCourseAsync(User, createDto.CourseId))
                return Forbid();

            if (!await IsValidCurriculumPlacement(createDto.CourseId, createDto.ModuleId, createDto.PrerequisiteLectureId, createDto.LectureId))
                return BadRequest(new { error = "Module and prerequisite lesson must belong to this course." });

            createDto.ActivityType = LearningActivityType.Normalize(createDto.ActivityType);

            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Update(string id, [FromBody] LectureUpdateDTO updateDto)
        {
            if (!await _courseAccessService.CanManageLectureAsync(User, id))
                return Forbid();

            var existing = await _lectureRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var courseId = updateDto.CourseId ?? existing.CourseId;
            if (string.IsNullOrWhiteSpace(courseId))
                return BadRequest(new { error = "CourseId is required." });

            if (!await IsValidCurriculumPlacement(courseId, updateDto.ModuleId, updateDto.PrerequisiteLectureId, id))
                return BadRequest(new { error = "Module and prerequisite lesson must belong to this course and cannot be itself." });

            updateDto.ActivityType = LearningActivityType.Normalize(updateDto.ActivityType);

            return await base.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public override async Task<IActionResult> Delete(string id)
        {
            if (!await _courseAccessService.CanManageLectureAsync(User, id))
                return Forbid();

            return await base.Delete(id);
        }

        private async Task<bool> IsValidCurriculumPlacement(
            string courseId,
            string? moduleId,
            string? prerequisiteLectureId,
            string currentLectureId)
        {
            if (!string.IsNullOrWhiteSpace(moduleId))
            {
                var moduleMatchesCourse = await _context.CourseModules
                    .AnyAsync(m => m.CourseId == courseId && m.ModuleId == moduleId);

                if (!moduleMatchesCourse)
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(prerequisiteLectureId))
            {
                if (prerequisiteLectureId == currentLectureId)
                    return false;

                var prerequisiteMatchesCourse = await _context.Lectures
                    .AnyAsync(l => l.CourseId == courseId && l.LectureId == prerequisiteLectureId);

                if (!prerequisiteMatchesCourse)
                    return false;
            }

            return true;
        }
    }
}
