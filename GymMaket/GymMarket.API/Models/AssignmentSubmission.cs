namespace GymMarket.API.Models;

public partial class AssignmentSubmission
{
    public string SubmissionId { get; set; } = null!;

    public string AssignmentId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string? TextResponse { get; set; }

    public string? AttachmentUrl { get; set; }

    public decimal? Score { get; set; }

    public decimal? ScorePercent { get; set; }

    public string Status { get; set; } = AssignmentSubmissionStatus.Submitted;

    public string? Feedback { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DateTime? GradedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CourseAssignment? Assignment { get; set; }

    public virtual Student? Student { get; set; }
}
