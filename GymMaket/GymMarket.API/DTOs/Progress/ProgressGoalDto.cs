namespace GymMarket.API.DTOs.Progress;

public class ProgressGoalDto
{
    public string ProgressGoalId { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public decimal? TargetWeightKg { get; set; }

    public decimal? TargetBodyFatPercent { get; set; }

    public DateTime? GoalDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
