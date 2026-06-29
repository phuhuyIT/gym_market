using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseModule;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseModuleController : ControllerBase
    {
        private readonly GymMarketContext _context;
        private readonly IMapper _mapper;
        private readonly ICourseAccessService _courseAccessService;

        public CourseModuleController(
            GymMarketContext context,
            IMapper mapper,
            ICourseAccessService courseAccessService)
        {
            _context = context;
            _mapper = mapper;
            _courseAccessService = courseAccessService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourse(string courseId)
        {
            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            var query = _context.CourseModules
                .AsNoTracking()
                .Where(m => m.CourseId == courseId);

            if (User.IsInRole(ApplicationRoles.Student))
                query = query.Where(m => m.IsPublished);

            var modules = await query
                .OrderBy(m => m.Order)
                .ThenBy(m => m.Title)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<CourseModuleDto>>(modules));
        }

        [HttpPost]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Create(CourseModuleCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _courseAccessService.CanManageCourseAsync(User, dto.CourseId))
                return Forbid();

            if (!await IsValidModulePrerequisite(dto.CourseId, dto.PrerequisiteModuleId, null))
                return BadRequest(new { error = "Prerequisite module must belong to this course." });

            var now = DateTime.UtcNow;
            var module = _mapper.Map<CourseModule>(dto);
            module.ModuleId = string.IsNullOrWhiteSpace(dto.ModuleId) ? Guid.NewGuid().ToString() : dto.ModuleId;
            module.CreatedAt = now;
            module.UpdatedAt = now;

            _context.CourseModules.Add(module);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<CourseModuleDto>(module));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Update(string id, CourseModuleUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var module = await _context.CourseModules.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
                return NotFound();

            var courseId = dto.CourseId ?? module.CourseId;
            if (string.IsNullOrWhiteSpace(courseId) || !await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            if (!await IsValidModulePrerequisite(courseId, dto.PrerequisiteModuleId, id))
                return BadRequest(new { error = "Prerequisite module must belong to this course and cannot be itself." });

            _mapper.Map(dto, module);
            module.ModuleId = id;
            module.CourseId = courseId;
            module.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<CourseModuleDto>(module));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var module = await _context.CourseModules.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(module.CourseId) || !await _courseAccessService.CanManageCourseAsync(User, module.CourseId))
                return Forbid();

            var dependentModuleExists = await _context.CourseModules
                .AnyAsync(m => m.PrerequisiteModuleId == id);
            if (dependentModuleExists)
                return Conflict(new { error = "Remove dependent module prerequisites before deleting this module." });

            var dependentLectureExists = await _context.Lectures
                .AnyAsync(l => l.ModuleId == id && l.PrerequisiteLectureId != null);

            _context.CourseModules.Remove(module);
            await _context.SaveChangesAsync();

            return dependentLectureExists
                ? Ok(new { message = "Module deleted. Review lesson prerequisites in the remaining curriculum." })
                : NoContent();
        }

        private async Task<bool> IsValidModulePrerequisite(string courseId, string? prerequisiteModuleId, string? currentModuleId)
        {
            if (string.IsNullOrWhiteSpace(prerequisiteModuleId))
                return true;

            if (prerequisiteModuleId == currentModuleId)
                return false;

            return await _context.CourseModules
                .AnyAsync(m => m.CourseId == courseId && m.ModuleId == prerequisiteModuleId);
        }
    }
}
