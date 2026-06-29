namespace GymMarket.API.Models;

public class StudentProgressGoal
{
    public string ProgressGoalId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public decimal? TargetWeightKg { get; set; }

    public decimal? TargetBodyFatPercent { get; set; }

    public DateTime? GoalDate { get; set; }

    public string Status { get; set; } = ProgressGoalStatus.Active;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual Student? Student { get; set; }
}
