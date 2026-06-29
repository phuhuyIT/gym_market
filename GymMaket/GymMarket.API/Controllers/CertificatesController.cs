using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Certificate;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly GymMarketContext _context;
        private readonly ICourseAccessService _courseAccessService;

        public CertificatesController(GymMarketContext context, ICourseAccessService courseAccessService)
        {
            _context = context;
            _courseAccessService = courseAccessService;
        }

        [HttpGet("course/{courseId}/completion")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCompletionStatus(string courseId)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            return Ok(await BuildCompletionStatus(courseId, studentId));
        }

        [HttpPost("course/{courseId}/issue")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Issue(string courseId)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            var status = await BuildCompletionStatus(courseId, studentId);
            if (status.Certificate != null)
                return Ok(status.Certificate);

            if (!status.IsCompleted)
                return Conflict(status);

            var certificate = new CourseCertificate
            {
                CertificateId = Guid.NewGuid().ToString(),
                CourseId = courseId,
                StudentId = studentId,
                VerificationCode = await CreateVerificationCode(),
                IssuedAt = DateTime.UtcNow
            };

            _context.CourseCertificates.Add(certificate);
            await _context.SaveChangesAsync();

            var created = await _context.CourseCertificates
                .AsNoTracking()
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstAsync(c => c.CertificateId == certificate.CertificateId);

            return Ok(ToCertificateDto(created));
        }

        [HttpGet("verify/{verificationCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify(string verificationCode)
        {
            var certificate = await _context.CourseCertificates
                .AsNoTracking()
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(c => c.VerificationCode == verificationCode);

            if (certificate == null)
                return NotFound(new { Message = "CERTIFICATE_NOT_FOUND" });

            return Ok(ToCertificateDto(certificate));
        }

        private async Task<CourseCompletionStatusDto> BuildCompletionStatus(string courseId, string studentId)
        {
            var lectureIds = await _context.Lectures
                .AsNoTracking()
                .Where(l => l.CourseId == courseId && l.IsPublished)
                .Select(l => l.LectureId)
                .ToListAsync();

            var completedLectureCount = await _context.LectureProgresses
                .AsNoTracking()
                .CountAsync(p => p.StudentId == studentId
                    && p.IsCompleted
                    && lectureIds.Contains(p.LectureId));

            var quiz = await _context.CourseQuizzes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.CourseId == courseId && q.IsPublished);

            var bestQuizAttempt = quiz == null
                ? null
                : await _context.QuizAttempts
                    .AsNoTracking()
                    .Where(a => a.QuizId == quiz.QuizId && a.StudentId == studentId)
                    .OrderByDescending(a => a.ScorePercent)
                    .ThenByDescending(a => a.SubmittedAt)
                    .FirstOrDefaultAsync();

            var certificate = await _context.CourseCertificates
                .AsNoTracking()
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.StudentId == studentId);

            var lecturesCompleted = lectureIds.Count > 0 && completedLectureCount == lectureIds.Count;
            var quizRequired = quiz != null;
            var quizPassed = !quizRequired || bestQuizAttempt?.Passed == true;

            return new CourseCompletionStatusDto
            {
                CourseId = courseId,
                TotalLectures = lectureIds.Count,
                CompletedLectures = completedLectureCount,
                LecturesCompleted = lecturesCompleted,
                QuizRequired = quizRequired,
                QuizPassed = quizPassed,
                BestQuizScorePercent = bestQuizAttempt?.ScorePercent,
                IsCompleted = lecturesCompleted && quizPassed,
                Certificate = certificate == null ? null : ToCertificateDto(certificate)
            };
        }

        private async Task<string> CreateVerificationCode()
        {
            for (var i = 0; i < 5; i++)
            {
                var code = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
                var exists = await _context.CourseCertificates.AnyAsync(c => c.VerificationCode == code);
                if (!exists)
                    return code;
            }

            return Guid.NewGuid().ToString("N").ToUpperInvariant();
        }

        private static CourseCertificateDto ToCertificateDto(CourseCertificate certificate)
        {
            return new CourseCertificateDto
            {
                CertificateId = certificate.CertificateId,
                CourseId = certificate.CourseId,
                CourseTitle = certificate.Course?.Title,
                StudentId = certificate.StudentId,
                StudentName = certificate.Student?.Name,
                VerificationCode = certificate.VerificationCode,
                IssuedAt = certificate.IssuedAt
            };
        }

        private string CurrentStudentId()
        {
            return User.FindFirstValue("studentId") ?? string.Empty;
        }
    }
}
