namespace GymMarket.API.DTOs.CourseAnalytics;

public class CourseAnalyticsDashboardDto
{
    public string CourseId { get; set; } = string.Empty;
    public string? CourseTitle { get; set; }
    public int TotalLearners { get; set; }
    public int TotalLectures { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalGradeItems { get; set; }
    public decimal AverageLessonProgressPercent { get; set; }
    public decimal? AverageCurrentGradePercent { get; set; }
    public decimal AverageFinalGradePercent { get; set; }
    public int CompletedLearners { get; set; }
    public int AtRiskLearners { get; set; }
    public decimal CompletionRatePercent { get; set; }
    public decimal AtRiskRatePercent { get; set; }
    public decimal SubmissionRatePercent { get; set; }
    public decimal AverageEngagementScore { get; set; }
    public CourseEngagementSummaryDto Engagement { get; set; } = new();
    public List<CoursePerformanceItemDto> PerformanceItems { get; set; } = [];
    public List<CourseAnalyticsTrendDto> Trends { get; set; } = [];
    public List<CourseLearnerAnalyticsDto> Learners { get; set; } = [];
}

public class MyCourseAnalyticsDto
{
    public string CourseId { get; set; } = string.Empty;
    public string? CourseTitle { get; set; }
    public CourseLearnerAnalyticsDto Progress { get; set; } = new();
}

public class CourseLearnerAnalyticsDto
{
    public string StudentId { get; set; } = string.Empty;
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public int TotalLectures { get; set; }
    public int CompletedLectures { get; set; }
    public decimal LessonProgressPercent { get; set; }
    public int TotalAssignments { get; set; }
    public int SubmittedAssignments { get; set; }
    public int GradedAssignments { get; set; }
    public int MissingAssignments { get; set; }
    public int TotalGradeItems { get; set; }
    public int CompletedGradeItems { get; set; }
    public decimal? CurrentGradePercent { get; set; }
    public decimal FinalGradePercent { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public DateTime? LastActivityAt { get; set; }
    public int QuizAttempts { get; set; }
    public int DiscussionPosts { get; set; }
    public bool CertificateIssued { get; set; }
    public int UpcomingItems { get; set; }
    public decimal EngagementScore { get; set; }
    public int RiskScore { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsAtRisk { get; set; }
    public List<string> AtRiskReasons { get; set; } = [];
}

public class CourseEngagementSummaryDto
{
    public int DiscussionQuestions { get; set; }
    public int DiscussionAnswers { get; set; }
    public int ActiveStudyGroups { get; set; }
    public int CertificatesIssued { get; set; }
    public int UpcomingCalendarItems { get; set; }
    public int ScheduledLiveSessions { get; set; }
}

public class CoursePerformanceItemDto
{
    public string ItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public decimal? AveragePercent { get; set; }
    public decimal PassRatePercent { get; set; }
    public int CompletedCount { get; set; }
    public int MissingCount { get; set; }
    public int TotalLearners { get; set; }
}

public class CourseAnalyticsTrendDto
{
    public DateTime WeekStart { get; set; }
    public int CompletedLessons { get; set; }
    public int AssignmentSubmissions { get; set; }
    public int QuizAttempts { get; set; }
    public int DiscussionPosts { get; set; }
}
