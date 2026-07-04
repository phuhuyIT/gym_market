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
        var quizzes = gradeItems.Where(i => i.ItemType == "Quiz").ToList();

        var progressByStudent = await LoadLectureProgressAsync(studentIds, lectureIds);
        var submissionsByStudent = await LoadAssignmentSubmissionsAsync(studentIds, assignments.Select(a => a.AssignmentId).ToList());
        var quizAttemptsByStudent = await LoadQuizAttemptsAsync(studentIds, quizzes.Select(q => q.ItemId).ToList());
        var discussionPostsByStudent = await LoadDiscussionPostsAsync(courseId, studentIds);
        var certificateStudentIds = await LoadCertificateStudentIdsAsync(courseId, studentIds);
        var upcomingItemCount = await CountUpcomingCalendarItemsAsync(courseId);
        var gradesByStudent = BuildGrades(studentIds, gradeItems, quizAttemptsByStudent, submissionsByStudent);
        var lastActivityByStudent = BuildLastActivity(progressByStudent, submissionsByStudent, quizAttemptsByStudent, discussionPostsByStudent, gradesByStudent);
        var performanceItems = BuildPerformanceItems(gradeItems, studentIds, quizAttemptsByStudent, submissionsByStudent);
        var engagement = await BuildEngagementSummaryAsync(courseId, upcomingItemCount);
        var trends = await BuildTrendsAsync(courseId);

        var learnerDtos = learners
            .Select(student => BuildLearnerAnalytics(
                student,
                lectures.Count,
                assignments.Count,
                gradeItems.Count,
                upcomingItemCount,
                progressByStudent,
                submissionsByStudent,
                quizAttemptsByStudent,
                discussionPostsByStudent,
                certificateStudentIds,
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
            CompletionRatePercent = learnerDtos.Count == 0 ? 0 : RoundGrade((decimal)learnerDtos.Count(l => l.IsCompleted) / learnerDtos.Count * 100),
            AtRiskRatePercent = learnerDtos.Count == 0 ? 0 : RoundGrade((decimal)learnerDtos.Count(l => l.IsAtRisk) / learnerDtos.Count * 100),
            SubmissionRatePercent = learnerDtos.Count == 0 || assignments.Count == 0
                ? 0
                : RoundGrade((decimal)learnerDtos.Sum(l => l.SubmittedAssignments) / (learnerDtos.Count * assignments.Count) * 100),
            AverageEngagementScore = AverageDecimal(learnerDtos.Select(l => (decimal?)l.EngagementScore)) ?? 0,
            Engagement = engagement,
            PerformanceItems = performanceItems,
            Trends = trends,
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
            quiz.Title,
            quiz.GradeCategoryId != null && validCategoryIds.Contains(quiz.GradeCategoryId) ? quiz.GradeCategoryId : defaultCategoryId,
            "Quiz")).ToList();

        var assignmentItems = assignments.Select(assignment => new GradeItem(
            assignment.AssignmentId,
            assignment.Title,
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

    private async Task<Dictionary<string, List<QuizAttempt>>> LoadQuizAttemptsAsync(List<string> studentIds, List<string> quizIds)
    {
        if (studentIds.Count == 0 || quizIds.Count == 0)
            return [];

        var attempts = await _context.QuizAttempts
            .AsNoTracking()
            .Where(a => quizIds.Contains(a.QuizId)
                && studentIds.Contains(a.StudentId)
                && a.Status != QuizAttemptStatus.PendingReview)
            .ToListAsync();

        return attempts.GroupBy(a => a.StudentId).ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task<Dictionary<string, int>> LoadDiscussionPostsAsync(string courseId, List<string> studentIds)
    {
        if (studentIds.Count == 0)
            return [];

        var questionCounts = await _context.CourseDiscussionQuestions
            .AsNoTracking()
            .Where(q => q.CourseId == courseId && studentIds.Contains(q.StudentId))
            .GroupBy(q => q.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToListAsync();

        var answerCounts = await _context.CourseDiscussionAnswers
            .AsNoTracking()
            .Where(a => a.Question.CourseId == courseId
                && a.AuthorRole == DiscussionAuthorRole.Student
                && a.AuthorEntityId != null
                && studentIds.Contains(a.AuthorEntityId))
            .GroupBy(a => a.AuthorEntityId!)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToListAsync();

        return questionCounts
            .Concat(answerCounts)
            .GroupBy(x => x.StudentId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));
    }

    private async Task<HashSet<string>> LoadCertificateStudentIdsAsync(string courseId, List<string> studentIds)
    {
        if (studentIds.Count == 0)
            return [];

        var rows = await _context.CourseCertificates
            .AsNoTracking()
            .Where(c => c.CourseId == courseId && studentIds.Contains(c.StudentId))
            .Select(c => c.StudentId)
            .ToListAsync();

        return rows.ToHashSet(StringComparer.Ordinal);
    }

    private static Dictionary<string, StudentGradeAnalytics> BuildGrades(
        List<string> studentIds,
        List<GradeItem> gradeItems,
        Dictionary<string, List<QuizAttempt>> quizAttemptsByStudent,
        Dictionary<string, List<AssignmentSubmission>> submissionsByStudent)
    {
        if (studentIds.Count == 0)
            return [];

        var gradeItemIds = gradeItems.Select(i => i.ItemId).ToList();

        return studentIds.ToDictionary(
            studentId => studentId,
            studentId =>
            {
                var scores = BuildScoreMap(studentId, quizAttemptsByStudent, submissionsByStudent);
                return BuildStudentGrade(studentId, gradeItems, gradeItemIds, scores);
            });
    }

    private static Dictionary<(string StudentId, string ItemId), GradeScore> BuildScoreMap(
        string studentId,
        Dictionary<string, List<QuizAttempt>> quizAttemptsByStudent,
        Dictionary<string, List<AssignmentSubmission>> submissionsByStudent)
    {
        var scores = new Dictionary<(string StudentId, string ItemId), GradeScore>();

        if (quizAttemptsByStudent.TryGetValue(studentId, out var attempts))
        {
            foreach (var attemptGroup in attempts.GroupBy(a => a.QuizId))
            {
                var best = attemptGroup
                    .OrderByDescending(a => a.ScorePercent)
                    .ThenByDescending(a => a.SubmittedAt)
                    .First();
                scores[(studentId, best.QuizId)] = new GradeScore(best.ScorePercent, best.SubmittedAt);
            }
        }

        if (submissionsByStudent.TryGetValue(studentId, out var submissions))
        {
            foreach (var submissionGroup in submissions.Where(s => s.ScorePercent.HasValue).GroupBy(s => s.AssignmentId))
            {
                var best = submissionGroup
                    .OrderByDescending(s => s.ScorePercent)
                    .ThenByDescending(s => s.GradedAt ?? s.SubmittedAt)
                    .First();
                scores[(studentId, best.AssignmentId)] = new GradeScore(best.ScorePercent!.Value, best.GradedAt ?? best.SubmittedAt);
            }
        }

        return scores;
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
        Dictionary<string, List<QuizAttempt>> quizAttemptsByStudent,
        Dictionary<string, int> discussionPostsByStudent,
        Dictionary<string, StudentGradeAnalytics> gradesByStudent)
    {
        var studentIds = progressByStudent.Keys
            .Concat(submissionsByStudent.Keys)
            .Concat(quizAttemptsByStudent.Keys)
            .Concat(discussionPostsByStudent.Keys)
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
            if (quizAttemptsByStudent.TryGetValue(studentId, out var attempts))
                dates.AddRange(attempts.Select(a => (DateTime?)a.SubmittedAt));
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
        int upcomingItems,
        Dictionary<string, List<LectureProgress>> progressByStudent,
        Dictionary<string, List<AssignmentSubmission>> submissionsByStudent,
        Dictionary<string, List<QuizAttempt>> quizAttemptsByStudent,
        Dictionary<string, int> discussionPostsByStudent,
        HashSet<string> certificateStudentIds,
        Dictionary<string, StudentGradeAnalytics> gradesByStudent,
        Dictionary<string, DateTime?> lastActivityByStudent)
    {
        progressByStudent.TryGetValue(student.StudentId, out var progressRows);
        submissionsByStudent.TryGetValue(student.StudentId, out var submissionRows);
        quizAttemptsByStudent.TryGetValue(student.StudentId, out var quizAttempts);
        discussionPostsByStudent.TryGetValue(student.StudentId, out var discussionPosts);
        gradesByStudent.TryGetValue(student.StudentId, out var grade);
        lastActivityByStudent.TryGetValue(student.StudentId, out var lastActivityAt);

        var completedLectures = progressRows?.Count(p => p.IsCompleted) ?? 0;
        var submittedAssignments = submissionRows?.Select(s => s.AssignmentId).Distinct().Count() ?? 0;
        var gradedAssignments = submissionRows?.Count(s => s.ScorePercent.HasValue) ?? 0;
        var lessonProgress = totalLectures == 0 ? 0 : RoundGrade((decimal)completedLectures / totalLectures * 100);
        var quizAttemptCount = quizAttempts?.Count ?? 0;
        var certificateIssued = certificateStudentIds.Contains(student.StudentId);
        var engagementScore = BuildEngagementScore(lessonProgress, submittedAssignments, totalAssignments, quizAttemptCount, discussionPosts, certificateIssued);
        var isCompleted = (totalLectures == 0 || completedLectures == totalLectures)
            && (totalAssignments == 0 || submittedAssignments == totalAssignments)
            && (totalGradeItems == 0 || grade?.CompletedGradeItems == totalGradeItems);

        var reasons = BuildAtRiskReasons(lessonProgress, totalAssignments - submittedAssignments, grade?.FinalPercent ?? 0, lastActivityAt);
        var riskScore = BuildRiskScore(reasons, lessonProgress, grade?.FinalPercent ?? 0, engagementScore);

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
            QuizAttempts = quizAttemptCount,
            DiscussionPosts = discussionPosts,
            CertificateIssued = certificateIssued,
            UpcomingItems = upcomingItems,
            EngagementScore = engagementScore,
            RiskScore = riskScore,
            RecommendedAction = RecommendedAction(reasons),
            IsCompleted = isCompleted,
            IsAtRisk = reasons.Count > 0,
            AtRiskReasons = reasons
        };
    }

    private List<CoursePerformanceItemDto> BuildPerformanceItems(
        List<GradeItem> gradeItems,
        List<string> studentIds,
        Dictionary<string, List<QuizAttempt>> quizAttemptsByStudent,
        Dictionary<string, List<AssignmentSubmission>> submissionsByStudent)
    {
        return gradeItems.Select(item =>
        {
            var scores = studentIds
                .Select(studentId => BestScoreForItem(studentId, item, quizAttemptsByStudent, submissionsByStudent))
                .Where(score => score.HasValue)
                .Select(score => score!.Value)
                .ToList();

            return new CoursePerformanceItemDto
            {
                ItemId = item.ItemId,
                Title = item.Title,
                ItemType = item.ItemType,
                AveragePercent = AverageDecimal(scores.Select(s => (decimal?)s)),
                PassRatePercent = scores.Count == 0 ? 0 : RoundGrade((decimal)scores.Count(s => s >= 60) / scores.Count * 100),
                CompletedCount = scores.Count,
                MissingCount = Math.Max(0, studentIds.Count - scores.Count),
                TotalLearners = studentIds.Count
            };
        })
        .OrderBy(item => item.ItemType)
        .ThenBy(item => item.Title)
        .ToList();
    }

    private static decimal? BestScoreForItem(
        string studentId,
        GradeItem item,
        Dictionary<string, List<QuizAttempt>> quizAttemptsByStudent,
        Dictionary<string, List<AssignmentSubmission>> submissionsByStudent)
    {
        if (item.ItemType == "Quiz")
        {
            return quizAttemptsByStudent.TryGetValue(studentId, out var attempts)
                ? attempts.Where(a => a.QuizId == item.ItemId).OrderByDescending(a => a.ScorePercent).Select(a => (decimal?)a.ScorePercent).FirstOrDefault()
                : null;
        }

        return submissionsByStudent.TryGetValue(studentId, out var submissions)
            ? submissions.Where(s => s.AssignmentId == item.ItemId && s.ScorePercent.HasValue).OrderByDescending(s => s.ScorePercent).Select(s => s.ScorePercent).FirstOrDefault()
            : null;
    }

    private async Task<CourseEngagementSummaryDto> BuildEngagementSummaryAsync(string courseId, int upcomingItemCount)
    {
        var discussionQuestions = await _context.CourseDiscussionQuestions
            .AsNoTracking()
            .CountAsync(q => q.CourseId == courseId);
        var discussionAnswers = await _context.CourseDiscussionAnswers
            .AsNoTracking()
            .CountAsync(a => a.Question.CourseId == courseId);
        var studyGroups = await _context.CourseStudyGroups
            .AsNoTracking()
            .CountAsync(g => g.CourseId == courseId && g.IsActive);
        var certificates = await _context.CourseCertificates
            .AsNoTracking()
            .CountAsync(c => c.CourseId == courseId);
        var liveSessions = await _context.CourseLiveSessions
            .AsNoTracking()
            .CountAsync(s => s.CourseId == courseId && s.Status == CourseLiveSessionStatus.Scheduled);

        return new CourseEngagementSummaryDto
        {
            DiscussionQuestions = discussionQuestions,
            DiscussionAnswers = discussionAnswers,
            ActiveStudyGroups = studyGroups,
            CertificatesIssued = certificates,
            UpcomingCalendarItems = upcomingItemCount,
            ScheduledLiveSessions = liveSessions
        };
    }

    private async Task<int> CountUpcomingCalendarItemsAsync(string courseId)
    {
        var now = DateTime.UtcNow;
        var cutoff = now.AddDays(14);
        var assignments = await _context.CourseAssignments
            .AsNoTracking()
            .CountAsync(a => a.CourseId == courseId && a.Status == AssignmentStatus.Published && a.DueAt > now && a.DueAt <= cutoff);
        var quizzes = await _context.CourseQuizzes
            .AsNoTracking()
            .CountAsync(q => q.CourseId == courseId && q.IsPublished && q.AvailableUntil > now && q.AvailableUntil <= cutoff);
        var sessions = await _context.CourseLiveSessions
            .AsNoTracking()
            .CountAsync(s => s.CourseId == courseId && s.Status == CourseLiveSessionStatus.Scheduled && s.StartsAt > now && s.StartsAt <= cutoff);

        return assignments + quizzes + sessions;
    }

    private async Task<List<CourseAnalyticsTrendDto>> BuildTrendsAsync(string courseId)
    {
        var start = DateTime.UtcNow.Date.AddDays(-35);
        var lectureIds = await _context.Lectures
            .AsNoTracking()
            .Where(l => l.CourseId == courseId)
            .Select(l => l.LectureId)
            .ToListAsync();
        var assignmentIds = await _context.CourseAssignments
            .AsNoTracking()
            .Where(a => a.CourseId == courseId)
            .Select(a => a.AssignmentId)
            .ToListAsync();
        var quizIds = await _context.CourseQuizzes
            .AsNoTracking()
            .Where(q => q.CourseId == courseId)
            .Select(q => q.QuizId)
            .ToListAsync();

        var lessonRows = lectureIds.Count == 0 ? [] : await _context.LectureProgresses
            .AsNoTracking()
            .Where(p => p.IsCompleted && lectureIds.Contains(p.LectureId) && (p.CompletedAt ?? p.UpdatedAt) >= start)
            .Select(p => p.CompletedAt ?? p.UpdatedAt)
            .ToListAsync();
        var submissionRows = assignmentIds.Count == 0 ? [] : await _context.AssignmentSubmissions
            .AsNoTracking()
            .Where(s => assignmentIds.Contains(s.AssignmentId) && s.SubmittedAt >= start)
            .Select(s => s.SubmittedAt)
            .ToListAsync();
        var attemptRows = quizIds.Count == 0 ? [] : await _context.QuizAttempts
            .AsNoTracking()
            .Where(a => quizIds.Contains(a.QuizId) && a.SubmittedAt >= start)
            .Select(a => a.SubmittedAt)
            .ToListAsync();
        var questionRows = await _context.CourseDiscussionQuestions
            .AsNoTracking()
            .Where(q => q.CourseId == courseId && q.CreatedAt >= start)
            .Select(q => q.CreatedAt)
            .ToListAsync();
        var answerRows = await _context.CourseDiscussionAnswers
            .AsNoTracking()
            .Where(a => a.Question.CourseId == courseId && a.CreatedAt >= start)
            .Select(a => a.CreatedAt)
            .ToListAsync();

        return Enumerable.Range(0, 6)
            .Select(offset => WeekStart(DateTime.UtcNow.Date.AddDays(-35 + offset * 7)))
            .Select(weekStart =>
            {
                var weekEnd = weekStart.AddDays(7);
                return new CourseAnalyticsTrendDto
                {
                    WeekStart = weekStart,
                    CompletedLessons = lessonRows.Count(d => d >= weekStart && d < weekEnd),
                    AssignmentSubmissions = submissionRows.Count(d => d >= weekStart && d < weekEnd),
                    QuizAttempts = attemptRows.Count(d => d >= weekStart && d < weekEnd),
                    DiscussionPosts = questionRows.Concat(answerRows).Count(d => d >= weekStart && d < weekEnd)
                };
            })
            .ToList();
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

    private static decimal BuildEngagementScore(
        decimal lessonProgress,
        int submittedAssignments,
        int totalAssignments,
        int quizAttempts,
        int discussionPosts,
        bool certificateIssued)
    {
        var assignmentScore = totalAssignments == 0 ? 20 : Math.Min(20, (decimal)submittedAssignments / totalAssignments * 20);
        var quizScore = Math.Min(15, quizAttempts * 5);
        var discussionScore = Math.Min(15, discussionPosts * 5);
        var certificateScore = certificateIssued ? 10 : 0;
        return RoundGrade(Math.Min(100, lessonProgress * 0.4m + assignmentScore + quizScore + discussionScore + certificateScore));
    }

    private static int BuildRiskScore(List<string> reasons, decimal lessonProgress, decimal finalGrade, decimal engagementScore)
    {
        var score = reasons.Count * 20;
        if (lessonProgress < 20) score += 15;
        if (finalGrade > 0 && finalGrade < 50) score += 15;
        if (engagementScore < 35) score += 10;
        return Math.Min(100, score);
    }

    private static string RecommendedAction(List<string> reasons)
    {
        if (reasons.Contains("No recent activity")) return "Send check-in";
        if (reasons.Contains("Missing assignments")) return "Review missing work";
        if (reasons.Contains("Grade below passing")) return "Schedule grade support";
        if (reasons.Contains("Low lesson progress")) return "Guide next lessons";
        return "Monitor progress";
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

    private static DateTime WeekStart(DateTime date)
    {
        var diff = ((int)date.DayOfWeek + 6) % 7;
        return date.Date.AddDays(-diff);
    }

    private string CurrentStudentId() => User.FindFirstValue("studentId") ?? string.Empty;

    private sealed record GradeItem(string ItemId, string Title, string CategoryId, string ItemType);
    private sealed record GradeScore(decimal ScorePercent, DateTime? GradedAt);
    private sealed record CategoryGradeAnalytics(string CategoryId, decimal? CurrentPercent, decimal FinalPercent, int GradedItems, int TotalItems);
    private sealed record StudentGradeAnalytics(decimal? CurrentPercent, decimal FinalPercent, string LetterGrade, int CompletedGradeItems, DateTime? LastGradeAt);
}
