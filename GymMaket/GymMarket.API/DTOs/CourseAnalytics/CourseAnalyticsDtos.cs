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
    public bool IsCompleted { get; set; }
    public bool IsAtRisk { get; set; }
    public List<string> AtRiskReasons { get; set; } = [];
}
