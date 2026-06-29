namespace GymMarket.API.DTOs.Progress;

public class ProgressSummaryDto
{
    public string StudentId { get; set; } = string.Empty;

    public string? StudentName { get; set; }

    public string? StudentEmail { get; set; }

    public int LogCount { get; set; }

    public DateTime? LatestLoggedAt { get; set; }

    public decimal? LatestWeightKg { get; set; }

    public decimal? WeightChangeKg { get; set; }

    public decimal? LatestBodyFatPercent { get; set; }

    public decimal? BodyFatChangePercent { get; set; }

    public ProgressGoalDto? ActiveGoal { get; set; }

    public string GoalStatusLabel { get; set; } = "No goal";
}
