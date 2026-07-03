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
    public List<AssignmentRubricCriterionDto> RubricCriteria { get; set; } = [];
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
    public List<UpsertAssignmentRubricCriterionDto> RubricCriteria { get; set; } = [];
}

public class SubmitAssignmentDto
{
    public string? TextResponse { get; set; }
    public string? AttachmentUrl { get; set; }
}

public class GradeAssignmentSubmissionDto
{
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public List<GradeAssignmentRubricScoreDto> RubricScores { get; set; } = [];
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
    public List<AssignmentRubricScoreDto> RubricScores { get; set; } = [];
}

public class AssignmentRubricCriterionDto
{
    public string CriterionId { get; set; } = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PointsPossible { get; set; }
    public int Order { get; set; }
}

public class UpsertAssignmentRubricCriterionDto
{
    public string? CriterionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PointsPossible { get; set; }
    public int Order { get; set; }
}

public class GradeAssignmentRubricScoreDto
{
    public string CriterionId { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string? Feedback { get; set; }
}

public class AssignmentRubricScoreDto
{
    public string RubricScoreId { get; set; } = string.Empty;
    public string SubmissionId { get; set; } = string.Empty;
    public string CriterionId { get; set; } = string.Empty;
    public string? CriterionTitle { get; set; }
    public decimal PointsPossible { get; set; }
    public decimal Score { get; set; }
    public string? Feedback { get; set; }
}
