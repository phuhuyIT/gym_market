using GymMarket.API.Data;
using GymMarket.API.DTOs.LectureProgress;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class LectureProgressController : ControllerBase
    {
        private readonly GymMarketContext _context;
        private readonly ICourseAccessService _courseAccessService;

        public LectureProgressController(GymMarketContext context, ICourseAccessService courseAccessService)
        {
            _context = context;
            _courseAccessService = courseAccessService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseProgress(string courseId)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            var lectureIds = await _context.Lectures
                .AsNoTracking()
                .Where(l => l.CourseId == courseId && l.IsPublished)
                .OrderBy(l => l.Order)
                .Select(l => l.LectureId)
                .ToListAsync();

            var completedLectureIds = await _context.LectureProgresses
                .AsNoTracking()
                .Where(p => p.StudentId == studentId
                    && p.IsCompleted
                    && lectureIds.Contains(p.LectureId))
                .Select(p => p.LectureId)
                .ToListAsync();

            var total = lectureIds.Count;
            var completed = completedLectureIds.Count;

            return Ok(new CourseProgressDto
            {
                CourseId = courseId,
                TotalLectures = total,
                CompletedLectures = completed,
                ProgressPercent = total == 0 ? 0 : (int)Math.Round((double)completed / total * 100),
                CompletedLectureIds = completedLectureIds
            });
        }

        [HttpPut("lecture/{lectureId}")]
        public async Task<IActionResult> UpdateLectureProgress(string lectureId, UpdateLectureProgressDto dto)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            if (!await _courseAccessService.CanAccessLectureAsync(User, lectureId))
                return Forbid();

            var unlockState = await _courseAccessService.GetLectureUnlockStateAsync(User, lectureId);
            if (unlockState.IsLocked)
                return Conflict(unlockState);

            var lectureExists = await _context.Lectures.AnyAsync(l => l.LectureId == lectureId);
            if (!lectureExists)
                return NotFound(new { Message = "LECTURE_NOT_FOUND" });

            var now = DateTime.UtcNow;
            var progress = await _context.LectureProgresses
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.LectureId == lectureId);

            if (progress == null)
            {
                progress = new LectureProgress
                {
                    LectureProgressId = Guid.NewGuid().ToString(),
                    StudentId = studentId,
                    LectureId = lectureId,
                    CreatedAt = now,
                };
                _context.LectureProgresses.Add(progress);
            }

            progress.IsCompleted = dto.IsCompleted;
            progress.CompletedAt = dto.IsCompleted ? now : null;
            progress.UpdatedAt = now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private string CurrentStudentId()
        {
            return User.FindFirstValue("studentId") ?? string.Empty;
        }
    }
}
