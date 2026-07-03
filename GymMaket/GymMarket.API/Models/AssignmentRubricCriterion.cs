namespace GymMarket.API.Models;

public partial class AssignmentRubricCriterion
{
    public string CriterionId { get; set; } = null!;

    public string AssignmentId { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal PointsPossible { get; set; }

    public int Order { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CourseAssignment? Assignment { get; set; }

    public virtual ICollection<AssignmentRubricScore> Scores { get; set; } = new List<AssignmentRubricScore>();
}
