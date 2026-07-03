namespace GymMarket.API.Models;

public partial class AssignmentRubricScore
{
    public string RubricScoreId { get; set; } = null!;

    public string SubmissionId { get; set; } = null!;

    public string CriterionId { get; set; } = null!;

    public decimal Score { get; set; }

    public string? Feedback { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AssignmentSubmission? Submission { get; set; }

    public virtual AssignmentRubricCriterion? Criterion { get; set; }
}
