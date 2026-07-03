namespace GymMarket.API.Models;

public partial class CourseAssignment
{
    public string AssignmentId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? GradeCategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Instructions { get; set; }

    public decimal PointsPossible { get; set; } = 100;

    public DateTime? DueAt { get; set; }

    public string SubmissionType { get; set; } = AssignmentSubmissionType.Text;

    public string Status { get; set; } = AssignmentStatus.Draft;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual GradeCategory? GradeCategory { get; set; }

    public virtual ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
}
