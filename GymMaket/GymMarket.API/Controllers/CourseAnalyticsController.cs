using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseAnalytics;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseAnalyticsController : ControllerBase
{
    private const string DefaultCategoryId = "analytics-default";
    private readonly GymMarketContext _context;
    private readonly ICourseAccessService _courseAccessService;

    public CourseAnalyticsController(GymMarketContext context, ICourseAccessService courseAccessService)
    {
        _context = context;
        _courseAccessService = courseAccessService;
    }

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetCourseDashboard(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var analytics = await BuildDashboardAsync(courseId);
        return analytics == null ? NotFound(new { Message = "COURSE_NOT_FOUND" }) : Ok(analytics);
    }

    [HttpGet("course/{courseId}/me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyCourseProgress(string courseId)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var analytics = await BuildDashboardAsync(courseId, studentId);
        if (analytics == null)
            return NotFound(new { Message = "COURSE_NOT_FOUND" });

        var learner = analytics.Learners.FirstOrDefault();
        if (learner == null)
            return NotFound(new { Message = "COURSE_PROGRESS_NOT_FOUND" });

        return Ok(new MyCourseAnalyticsDto
        {
            CourseId = analytics.CourseId,
            CourseTitle = analytics.CourseTitle,
            Progress = learner
        });
    }

    private async Task<CourseAnalyticsDashboardDto?> BuildDashboardAsync(string courseId, string? onlyStudentId = null)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course == null)
            return null;

        var learners = await LoadPaidStudentsAsync(courseId, onlyStudentId);
        var studentIds = learners.Select(s => s.StudentId).ToList();
        var lectures = await LoadPublishedLecturesAsync(courseId);
        var lectureIds = lectures.Select(l => l.LectureId).ToList();
        var assignments = await LoadPublishedAssignmentsAsync(courseId);
        var gradeItems = await LoadGradeItemsAsync(courseId, assignments);

        var progressByStudent = await LoadLectureProgressAsync(studentIds, lectureIds);
        var submissionsByStudent = await LoadAssignmentSubmissionsAsync(studentIds, assignments.Select(a => a.AssignmentId).ToList());
        var gradesByStudent = await BuildGradesAsync(studentIds, gradeItems);
        var lastActivityByStudent = BuildLastActivity(progressByStudent, submissionsByStudent, gradesByStudent);

        var learnerDtos = learners
            .Select(student => BuildLearnerAnalytics(
                student,
                lectures.Count,
                assignments.Count,
                gradeItems.Count,
                progressByStudent,
                submissionsByStudent,
                gradesByStudent,
                lastActivityByStudent))
            .OrderByDescending(l => l.IsAtRisk)
            .ThenBy(l => l.StudentName ?? l.StudentEmail ?? l.StudentId)
            .ToList();

        return new CourseAnalyticsDashboardDto
        {
            CourseId = course.CourseId,
            CourseTitle = course.Title,
            TotalLearners = learnerDtos.Count,
            TotalLectures = lectures.Count,
            TotalAssignments = assignments.Count,
            TotalGradeItems = gradeItems.Count,
            AverageLessonProgressPercent = AverageDecimal(learnerDtos.Select(l => (decimal?)l.LessonProgressPercent)) ?? 0,
            AverageCurrentGradePercent = AverageDecimal(learnerDtos.Select(l => l.CurrentGradePercent)),
            AverageFinalGradePercent = AverageDecimal(learnerDtos.Select(l => (decimal?)l.FinalGradePercent)) ?? 0,
            CompletedLearners = learnerDtos.Count(l => l.IsCompleted),
            AtRiskLearners = learnerDtos.Count(l => l.IsAtRisk),
            Learners = learnerDtos
        };
    }

    private async Task<List<Student>> LoadPaidStudentsAsync(string courseId, string? onlyStudentId)
    {
        var registrationStudents = await _context.CourseRegistrations
            .AsNoTracking()
            .Include(r => r.Student)
            .Where(r => r.CourseId == courseId
                && (r.PaymentStatus == PaymentStatus.Paid || r.PaymentStatus == PaymentStatus.Completed)
                && r.Student != null
                && (onlyStudentId == null || r.StudentId == onlyStudentId))
            .Select(r => r.Student!)
            .ToListAsync();

        var paymentStudents = await _context.Payments
            .AsNoTracking()
            .Include(p => p.Student)
            .Where(p => p.CourseId == courseId
                && (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed)
                && p.Student != null
                && (onlyStudentId == null || p.StudentId == onlyStudentId))
            .Select(p => p.Student!)
            .ToListAsync();

        return registrationStudents
            .Concat(paymentStudents)
            .GroupBy(s => s.StudentId)
            .Select(g => g.First())
            .OrderBy(s => s.Name ?? s.Email ?? s.StudentId)
            .ToList();
    }

    private async Task<List<Lecture>> LoadPublishedLecturesAsync(string courseId) =>
        await _context.Lectures
            .AsNoTracking()
            .Where(l => l.CourseId == courseId && l.IsPublished)
            .OrderBy(l => l.Module != null ? l.Module.Order : 0)
            .ThenBy(l => l.Order)
            .ToListAsync();

    private async Task<List<CourseAssignment>> LoadPublishedAssignmentsAsync(string courseId) =>
        await _context.CourseAssignments
            .AsNoTracking()
            .Where(a => a.CourseId == courseId && a.Status == AssignmentStatus.Published)
            .OrderBy(a => a.DueAt ?? DateTime.MaxValue)
            .ThenBy(a => a.Title)
            .ToListAsync();

    private async Task<List<GradeItem>> LoadGradeItemsAsync(string courseId, List<CourseAssignment> assignments)
    {
        var categories = await _context.GradeCategories
            .AsNoTracking()
            .Where(c => c.CourseId == courseId)
            .OrderBy(c => c.Order)
            .ToListAsync();
        var defaultCategoryId = categories.FirstOrDefault()?.CategoryId ?? DefaultCategoryId;
        var validCategoryIds = categories.Select(c => c.CategoryId).ToHashSet(StringComparer.Ordinal);

        var quizzes = await _context.CourseQuizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .Where(q => q.CourseId == courseId && q.IsPublished)
            .ToListAsync();

        var quizItems = quizzes.Select(quiz => new GradeItem(
            quiz.QuizId,
            quiz.GradeCategoryId != null && validCategoryIds.Contains(quiz.GradeCategoryId) ? quiz.GradeCategoryId : defaultCategoryId,
            "Quiz")).ToList();

        var assignmentItems = assignments.Select(assignment => new GradeItem(
            assignment.AssignmentId,
            assignment.GradeCategoryId != null && validCategoryIds.Contains(assignment.GradeCategoryId) ? assignment.GradeCategoryId : defaultCategoryId,
            "Assignment")).ToList();

        return quizItems.Concat(assignmentItems).ToList();
    }

    private async Task<Dictionary<string, List<LectureProgress>>> LoadLectureProgressAsync(List<string> studentIds, List<string> lectureIds)
    {
        if (studentIds.Count == 0 || lectureIds.Count == 0)
            return [];

        var rows = await _context.LectureProgresses
            .AsNoTracking()
            .Where(p => studentIds.Contains(p.StudentId) && lectureIds.Contains(p.LectureId))
            .ToListAsync();

        return rows.GroupBy(p => p.StudentId).ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task<Dictionary<string, List<AssignmentSubmission>>> LoadAssignmentSubmissionsAsync(List<string> studentIds, List<string> assignmentIds)
    {
        if (studentIds.Count == 0 || assignmentIds.Count == 0)
            return [];

        var rows = await _context.AssignmentSubmissions
            .AsNoTracking()
            .Where(s => studentIds.Contains(s.StudentId) && assignmentIds.Contains(s.AssignmentId))
            .ToListAsync();

        return rows.GroupBy(s => s.StudentId).ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task<Dictionary<string, StudentGradeAnalytics>> BuildGradesAsync(List<string> studentIds, List<GradeItem> gradeItems)
    {
        if (studentIds.Count == 0)
            return [];

        var gradeItemIds = gradeItems.Select(i => i.ItemId).ToList();
        var quizIds = gradeItems.Where(i => i.ItemType == "Quiz").Select(i => i.ItemId).ToList();
        var assignmentIds = gradeItems.Where(i => i.ItemType == "Assignment").Select(i => i.ItemId).ToList();
        var scores = new Dictionary<(string StudentId, string ItemId), GradeScore>();

        if (quizIds.Count > 0)
        {
            var attempts = await _context.QuizAttempts
                .AsNoTracking()
                .Where(a => quizIds.Contains(a.QuizId)
                    && studentIds.Contains(a.StudentId)
                    && a.Status != QuizAttemptStatus.PendingReview)
                .ToListAsync();

            foreach (var attemptGroup in attempts.GroupBy(a => new { a.StudentId, a.QuizId }))
            {
                var best = attemptGroup
                    .OrderByDescending(a => a.ScorePercent)
                    .ThenByDescending(a => a.SubmittedAt)
                    .First();
                scores[(best.StudentId, best.QuizId)] = new GradeScore(best.ScorePercent, best.SubmittedAt);
            }
        }

        if (assignmentIds.Count > 0)
        {
            var submissions = await _context.AssignmentSubmissions
                .AsNoTracking()
                .Where(s => assignmentIds.Contains(s.AssignmentId)
                    && studentIds.Contains(s.StudentId)
                    && s.ScorePercent.HasValue)
                .ToListAsync();

            foreach (var submission in submissions)
            {
                scores[(submission.StudentId, submission.AssignmentId)] = new GradeScore(submission.ScorePercent!.Value, submission.GradedAt ?? submission.SubmittedAt);
            }
        }

        return studentIds.ToDictionary(
            studentId => studentId,
            studentId => BuildStudentGrade(studentId, gradeItems, gradeItemIds, scores));
    }

    private static StudentGradeAnalytics BuildStudentGrade(
        string studentId,
        List<GradeItem> gradeItems,
        List<string> gradeItemIds,
        Dictionary<(string StudentId, string ItemId), GradeScore> scores)
    {
        if (gradeItems.Count == 0)
            return new StudentGradeAnalytics(null, 0, string.Empty, 0, null);

        var completed = gradeItemIds.Count(itemId => scores.ContainsKey((studentId, itemId)));
        var categoryGrades = new List<CategoryGradeAnalytics>();

        foreach (var categoryGroup in gradeItems.GroupBy(i => i.CategoryId))
        {
            var itemIds = categoryGroup.Select(i => i.ItemId).ToList();
            var gradedScores = itemIds
                .Where(itemId => scores.ContainsKey((studentId, itemId)))
                .Select(itemId => scores[(studentId, itemId)].ScorePercent)
                .ToList();

            var current = AverageDecimal(gradedScores.Select(s => (decimal?)s));
            var final = itemIds.Count == 0
                ? 0
                : RoundGrade(itemIds.Average(itemId => scores.TryGetValue((studentId, itemId), out var score) ? score.ScorePercent : 0));

            categoryGrades.Add(new CategoryGradeAnalytics(categoryGroup.Key, current, final, gradedScores.Count, itemIds.Count));
        }

        var gradedCategories = categoryGrades.Where(c => c.GradedItems > 0).ToList();
        decimal? currentPercent = gradedCategories.Count == 0
            ? null
            : RoundGrade(gradedCategories.Average(c => c.CurrentPercent ?? 0));
        var finalPercent = RoundGrade(categoryGrades.Average(c => c.FinalPercent));
        var lastGradeAt = scores
            .Where(s => s.Key.StudentId == studentId)
            .Select(s => s.Value.GradedAt)
            .DefaultIfEmpty()
            .Max();

        return new StudentGradeAnalytics(currentPercent, finalPercent, ToLetterGrade(finalPercent), completed, lastGradeAt);
    }

    private static Dictionary<string, DateTime?> BuildLastActivity(
        Dictionary<string, List<LectureProgress>> progressByStudent,
        Dictionary<string, List<AssignmentSubmission>> submissionsByStudent,
        Dictionary<string, StudentGradeAnalytics> gradesByStudent)
    {
        var studentIds = progressByStudent.Keys
            .Concat(submissionsByStudent.Keys)
            .Concat(gradesByStudent.Keys)
            .Distinct()
            .ToList();

        return studentIds.ToDictionary(studentId => studentId, studentId =>
        {
            var dates = new List<DateTime?>();
            if (progressByStudent.TryGetValue(studentId, out var progresses))
                dates.AddRange(progresses.Select(p => (DateTime?)(p.CompletedAt ?? p.UpdatedAt)));
            if (submissionsByStudent.TryGetValue(studentId, out var submissions))
                dates.AddRange(submissions.Select(s => (DateTime?)(s.GradedAt ?? s.UpdatedAt)));
            if (gradesByStudent.TryGetValue(studentId, out var grade))
                dates.Add(grade.LastGradeAt);

            return dates.Where(d => d.HasValue).DefaultIfEmpty().Max();
        });
    }

    private static CourseLearnerAnalyticsDto BuildLearnerAnalytics(
        Student student,
        int totalLectures,
        int totalAssignments,
        int totalGradeItems,
        Dictionary<string, List<LectureProgress>> progressByStudent,
        Dictionary<string, List<AssignmentSubmission>> submissionsByStudent,
        Dictionary<string, StudentGradeAnalytics> gradesByStudent,
        Dictionary<string, DateTime?> lastActivityByStudent)
    {
        progressByStudent.TryGetValue(student.StudentId, out var progressRows);
        submissionsByStudent.TryGetValue(student.StudentId, out var submissionRows);
        gradesByStudent.TryGetValue(student.StudentId, out var grade);
        lastActivityByStudent.TryGetValue(student.StudentId, out var lastActivityAt);

        var completedLectures = progressRows?.Count(p => p.IsCompleted) ?? 0;
        var submittedAssignments = submissionRows?.Select(s => s.AssignmentId).Distinct().Count() ?? 0;
        var gradedAssignments = submissionRows?.Count(s => s.ScorePercent.HasValue) ?? 0;
        var lessonProgress = totalLectures == 0 ? 0 : RoundGrade((decimal)completedLectures / totalLectures * 100);
        var isCompleted = (totalLectures == 0 || completedLectures == totalLectures)
            && (totalAssignments == 0 || submittedAssignments == totalAssignments)
            && (totalGradeItems == 0 || grade?.CompletedGradeItems == totalGradeItems);

        var reasons = BuildAtRiskReasons(lessonProgress, totalAssignments - submittedAssignments, grade?.FinalPercent ?? 0, lastActivityAt);

        return new CourseLearnerAnalyticsDto
        {
            StudentId = student.StudentId,
            StudentName = student.Name,
            StudentEmail = student.Email,
            TotalLectures = totalLectures,
            CompletedLectures = completedLectures,
            LessonProgressPercent = lessonProgress,
            TotalAssignments = totalAssignments,
            SubmittedAssignments = submittedAssignments,
            GradedAssignments = gradedAssignments,
            MissingAssignments = Math.Max(0, totalAssignments - submittedAssignments),
            TotalGradeItems = totalGradeItems,
            CompletedGradeItems = grade?.CompletedGradeItems ?? 0,
            CurrentGradePercent = grade?.CurrentPercent,
            FinalGradePercent = grade?.FinalPercent ?? 0,
            LetterGrade = grade?.LetterGrade ?? string.Empty,
            LastActivityAt = lastActivityAt,
            IsCompleted = isCompleted,
            IsAtRisk = reasons.Count > 0,
            AtRiskReasons = reasons
        };
    }

    private static List<string> BuildAtRiskReasons(decimal lessonProgress, int missingAssignments, decimal finalGrade, DateTime? lastActivityAt)
    {
        var reasons = new List<string>();
        if (lessonProgress < 40)
            reasons.Add("Low lesson progress");
        if (missingAssignments > 0)
            reasons.Add("Missing assignments");
        if (finalGrade > 0 && finalGrade < 60)
            reasons.Add("Grade below passing");
        if (!lastActivityAt.HasValue || lastActivityAt.Value < DateTime.UtcNow.AddDays(-14))
            reasons.Add("No recent activity");

        return reasons;
    }

    private static decimal? AverageDecimal(IEnumerable<decimal?> values)
    {
        var present = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
        return present.Count == 0 ? null : RoundGrade(present.Average());
    }

    private static decimal RoundGrade(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static string ToLetterGrade(decimal percent)
    {
        if (percent >= 90) return "A";
        if (percent >= 80) return "B";
        if (percent >= 70) return "C";
        if (percent >= 60) return "D";
        return "F";
    }

    private string CurrentStudentId() => User.FindFirstValue("studentId") ?? string.Empty;

    private sealed record GradeItem(string ItemId, string CategoryId, string ItemType);
    private sealed record GradeScore(decimal ScorePercent, DateTime? GradedAt);
    private sealed record CategoryGradeAnalytics(string CategoryId, decimal? CurrentPercent, decimal FinalPercent, int GradedItems, int TotalItems);
    private sealed record StudentGradeAnalytics(decimal? CurrentPercent, decimal FinalPercent, string LetterGrade, int CompletedGradeItems, DateTime? LastGradeAt);
}
