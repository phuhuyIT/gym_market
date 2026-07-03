namespace GymMarket.API.Models;

public partial class GradeCategory
{
    public string CategoryId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal WeightPercent { get; set; }

    public int Order { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<CourseQuiz> CourseQuizzes { get; set; } = new List<CourseQuiz>();

    public virtual ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
}
