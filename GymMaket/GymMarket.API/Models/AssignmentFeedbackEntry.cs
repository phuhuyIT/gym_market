namespace GymMarket.API.Models;

public partial class AssignmentFeedbackEntry
{
    public string FeedbackEntryId { get; set; } = null!;

    public string SubmissionId { get; set; } = null!;

    public string? AuthorUserId { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string AuthorRole { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal? Score { get; set; }

    public decimal? ScorePercent { get; set; }

    public string? Feedback { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AssignmentSubmission? Submission { get; set; }
}
