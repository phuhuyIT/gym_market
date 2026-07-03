namespace GymMarket.API.DTOs.Assignments;

public class CourseAssignmentDto
{
    public string AssignmentId { get; set; } = string.Empty;
    public string CourseId { get; set; } = string.Empty;
    public string? GradeCategoryId { get; set; }
    public string? GradeCategoryName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public decimal PointsPossible { get; set; }
    public DateTime? DueAt { get; set; }
    public string SubmissionType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int SubmissionCount { get; set; }
    public int GradedCount { get; set; }
    public AssignmentSubmissionDto? MySubmission { get; set; }
}

public class UpsertCourseAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public string? GradeCategoryId { get; set; }
    public decimal PointsPossible { get; set; } = 100;
    public DateTime? DueAt { get; set; }
    public string? SubmissionType { get; set; }
    public string? Status { get; set; }
}

public class SubmitAssignmentDto
{
    public string? TextResponse { get; set; }
    public string? AttachmentUrl { get; set; }
}

public class GradeAssignmentSubmissionDto
{
    public decimal Score { get; set; }
    public string? Feedback { get; set; }
}

public class AssignmentSubmissionDto
{
    public string SubmissionId { get; set; } = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public string? TextResponse { get; set; }
    public string? AttachmentUrl { get; set; }
    public decimal? Score { get; set; }
    public decimal? ScorePercent { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Feedback { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? GradedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
