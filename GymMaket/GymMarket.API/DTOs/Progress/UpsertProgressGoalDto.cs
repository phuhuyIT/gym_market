namespace GymMarket.API.DTOs.Progress;

public class UpsertProgressGoalDto
{
    public decimal? TargetWeightKg { get; set; }

    public decimal? TargetBodyFatPercent { get; set; }

    public DateTime? GoalDate { get; set; }

    public string Status { get; set; } = "Active";

    public string? Notes { get; set; }
}
