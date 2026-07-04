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

        [HttpGet("course/{courseId}/settings")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GetSettings(string courseId)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var course = await _context.Courses
                .AsNoTracking()
                .Include(c => c.CertificateSetting)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
                return NotFound(new { Message = "COURSE_NOT_FOUND" });

            return Ok(ToSettingDto(GetSetting(course), course));
        }

        [HttpPut("course/{courseId}/settings")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> UpdateSettings(string courseId, UpdateCourseCertificateSettingDto dto)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var course = await _context.Courses
                .Include(c => c.CertificateSetting)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
                return NotFound(new { Message = "COURSE_NOT_FOUND" });

            var validationError = ValidateSettings(dto);
            if (validationError != null)
                return BadRequest(new { Message = validationError });

            var now = DateTime.UtcNow;
            var setting = course.CertificateSetting;
            if (setting == null)
            {
                setting = new CourseCertificateSetting
                {
                    CourseId = courseId,
                    CreatedAt = now
                };
                _context.CourseCertificateSettings.Add(setting);
            }

            setting.IsEnabled = dto.IsEnabled;
            setting.TemplateName = dto.TemplateName.Trim();
            setting.CertificateTitle = dto.CertificateTitle.Trim();
            setting.BodyText = dto.BodyText.Trim();
            setting.AccentColor = dto.AccentColor.Trim();
            setting.RequiredLecturePercent = dto.RequiredLecturePercent;
            setting.RequirePublishedQuizzes = dto.RequirePublishedQuizzes;
            setting.RequirePublishedAssignments = dto.RequirePublishedAssignments;
            setting.RequiredAssignmentPercent = dto.RequiredAssignmentPercent;
            setting.MinimumFinalGradePercent = dto.MinimumFinalGradePercent;
            setting.UpdatedAt = now;

            await _context.SaveChangesAsync();

            return Ok(ToSettingDto(setting, course));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCertificates()
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            var certificates = await _context.CourseCertificates
                .AsNoTracking()
                .Include(c => c.Course)
                    .ThenInclude(c => c!.CertificateSetting)
                .Include(c => c.Student)
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.IssuedAt)
                .ToListAsync();

            return Ok(certificates.Select(ToCertificateDto));
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
                    .ThenInclude(c => c!.CertificateSetting)
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
                    .ThenInclude(c => c!.CertificateSetting)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(c => c.VerificationCode == verificationCode);

            if (certificate == null)
                return NotFound(new { Message = "CERTIFICATE_NOT_FOUND" });

            return Ok(ToCertificateDto(certificate));
        }

        private async Task<CourseCompletionStatusDto> BuildCompletionStatus(string courseId, string studentId)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .Include(c => c.CertificateSetting)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            var setting = GetSetting(course, courseId);

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

            var quizzes = await _context.CourseQuizzes
                .AsNoTracking()
                .Where(q => q.CourseId == courseId && q.IsPublished)
                .ToListAsync();

            var quizIds = quizzes.Select(q => q.QuizId).ToList();
            var attempts = quizIds.Count == 0
                ? new List<QuizAttempt>()
                : await _context.QuizAttempts
                    .AsNoTracking()
                    .Where(a => quizIds.Contains(a.QuizId) && a.StudentId == studentId)
                    .OrderByDescending(a => a.ScorePercent)
                    .ThenByDescending(a => a.SubmittedAt)
                    .ToListAsync();

            var bestQuizAttempt = attempts
                .OrderByDescending(a => a.ScorePercent)
                .ThenByDescending(a => a.SubmittedAt)
                .FirstOrDefault();

            var certificate = await _context.CourseCertificates
                .AsNoTracking()
                .Include(c => c.Course)
                    .ThenInclude(c => c!.CertificateSetting)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.StudentId == studentId);

            var assignments = await _context.CourseAssignments
                .AsNoTracking()
                .Where(a => a.CourseId == courseId && a.Status == AssignmentStatus.Published)
                .ToListAsync();

            var assignmentIds = assignments.Select(a => a.AssignmentId).ToList();
            var submissions = assignmentIds.Count == 0
                ? new List<AssignmentSubmission>()
                : await _context.AssignmentSubmissions
                    .AsNoTracking()
                    .Where(s => assignmentIds.Contains(s.AssignmentId)
                        && s.StudentId == studentId
                        && s.ScorePercent.HasValue)
                    .ToListAsync();

            var bestAssignmentScores = submissions
                .GroupBy(s => s.AssignmentId)
                .Select(g => g.OrderByDescending(s => s.ScorePercent).ThenByDescending(s => s.GradedAt ?? s.SubmittedAt).First())
                .ToList();
            var assignmentAveragePercent = AverageNullable(bestAssignmentScores.Select(s => s.ScorePercent));
            var assignmentRequired = setting.RequirePublishedAssignments && assignments.Count > 0;
            var assignmentPassed = !assignmentRequired
                || (bestAssignmentScores.Count == assignments.Count
                    && assignmentAveragePercent.HasValue
                    && assignmentAveragePercent.Value >= setting.RequiredAssignmentPercent);

            var quizScorePercents = attempts
                .Where(a => a.Status != QuizAttemptStatus.PendingReview)
                .GroupBy(a => a.QuizId)
                .Select(g => (decimal?)g.OrderByDescending(a => a.ScorePercent).ThenByDescending(a => a.SubmittedAt).First().ScorePercent)
                .ToList();
            var finalGradePercent = AverageNullable(quizScorePercents.Concat(bestAssignmentScores.Select(s => s.ScorePercent)));
            var finalGradePassed = !setting.MinimumFinalGradePercent.HasValue
                || (finalGradePercent.HasValue && finalGradePercent.Value >= setting.MinimumFinalGradePercent.Value);

            var lectureCompletionPercent = lectureIds.Count == 0
                ? 0m
                : Math.Round(completedLectureCount * 100m / lectureIds.Count, 2);
            var lecturesCompleted = lectureIds.Count > 0 && lectureCompletionPercent >= setting.RequiredLecturePercent;
            var quizRequired = setting.RequirePublishedQuizzes && quizzes.Count > 0;
            var quizPassed = !quizRequired || quizzes.All(quiz =>
                attempts.Any(attempt => attempt.QuizId == quiz.QuizId
                    && attempt.Passed
                    && attempt.Status != QuizAttemptStatus.PendingReview));

            return new CourseCompletionStatusDto
            {
                CourseId = courseId,
                TotalLectures = lectureIds.Count,
                CompletedLectures = completedLectureCount,
                LecturesCompleted = lecturesCompleted,
                LectureCompletionPercent = lectureCompletionPercent,
                RequiredLecturePercent = setting.RequiredLecturePercent,
                QuizRequired = quizRequired,
                QuizPassed = quizPassed,
                BestQuizScorePercent = bestQuizAttempt?.ScorePercent,
                AssignmentRequired = assignmentRequired,
                AssignmentPassed = assignmentPassed,
                TotalAssignments = assignments.Count,
                GradedAssignments = bestAssignmentScores.Count,
                AssignmentAveragePercent = assignmentAveragePercent,
                RequiredAssignmentPercent = setting.RequiredAssignmentPercent,
                FinalGradePercent = finalGradePercent,
                MinimumFinalGradePercent = setting.MinimumFinalGradePercent,
                FinalGradePassed = finalGradePassed,
                CertificatesEnabled = setting.IsEnabled,
                Setting = ToSettingDto(setting, course),
                IsCompleted = setting.IsEnabled && lecturesCompleted && quizPassed && assignmentPassed && finalGradePassed,
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
                IssuedAt = certificate.IssuedAt,
                Setting = ToSettingDto(GetSetting(certificate.Course, certificate.CourseId), certificate.Course)
            };
        }

        private static CourseCertificateSetting GetSetting(Course? course, string? courseId = null) =>
            course?.CertificateSetting ?? new CourseCertificateSetting
            {
                CourseId = course?.CourseId ?? courseId ?? string.Empty,
                IsEnabled = true,
                TemplateName = "Classic",
                CertificateTitle = "Certificate of Completion",
                BodyText = "Awarded for successfully completing this course.",
                AccentColor = "#007AFF",
                RequiredLecturePercent = 100m,
                RequirePublishedQuizzes = true,
                RequirePublishedAssignments = false,
                RequiredAssignmentPercent = 0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

        private static CourseCertificateSettingDto ToSettingDto(CourseCertificateSetting setting, Course? course) => new()
        {
            CourseId = setting.CourseId,
            CourseTitle = course?.Title,
            IsEnabled = setting.IsEnabled,
            TemplateName = setting.TemplateName,
            CertificateTitle = setting.CertificateTitle,
            BodyText = setting.BodyText,
            AccentColor = setting.AccentColor,
            RequiredLecturePercent = setting.RequiredLecturePercent,
            RequirePublishedQuizzes = setting.RequirePublishedQuizzes,
            RequirePublishedAssignments = setting.RequirePublishedAssignments,
            RequiredAssignmentPercent = setting.RequiredAssignmentPercent,
            MinimumFinalGradePercent = setting.MinimumFinalGradePercent,
            UpdatedAt = setting.UpdatedAt
        };

        private static string? ValidateSettings(UpdateCourseCertificateSettingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TemplateName))
                return "CERTIFICATE_TEMPLATE_NAME_REQUIRED";
            if (string.IsNullOrWhiteSpace(dto.CertificateTitle))
                return "CERTIFICATE_TITLE_REQUIRED";
            if (string.IsNullOrWhiteSpace(dto.BodyText))
                return "CERTIFICATE_BODY_REQUIRED";
            if (dto.TemplateName.Length > 80)
                return "CERTIFICATE_TEMPLATE_NAME_TOO_LONG";
            if (dto.CertificateTitle.Length > 160)
                return "CERTIFICATE_TITLE_TOO_LONG";
            if (dto.BodyText.Length > 1000)
                return "CERTIFICATE_BODY_TOO_LONG";
            if (dto.RequiredLecturePercent < 0 || dto.RequiredLecturePercent > 100)
                return "CERTIFICATE_LECTURE_REQUIREMENT_INVALID";
            if (dto.RequiredAssignmentPercent < 0 || dto.RequiredAssignmentPercent > 100)
                return "CERTIFICATE_ASSIGNMENT_REQUIREMENT_INVALID";
            if (dto.MinimumFinalGradePercent is < 0 or > 100)
                return "CERTIFICATE_GRADE_REQUIREMENT_INVALID";

            return null;
        }

        private static decimal? AverageNullable(IEnumerable<decimal?> values)
        {
            var materialized = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
            return materialized.Count == 0 ? null : Math.Round(materialized.Average(), 2);
        }

        private string CurrentStudentId()
        {
            return User.FindFirstValue("studentId") ?? string.Empty;
        }
    }
}
