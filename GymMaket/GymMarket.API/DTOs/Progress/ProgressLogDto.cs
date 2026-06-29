namespace GymMarket.API.DTOs.Progress;

public class ProgressLogDto
{
    public string ProgressLogId { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string? StudentName { get; set; }

    public string? StudentEmail { get; set; }

    public DateTime LoggedAt { get; set; }

    public decimal? WeightKg { get; set; }

    public decimal? BodyFatPercent { get; set; }

    public decimal? WaistCm { get; set; }

    public decimal? ChestCm { get; set; }

    public decimal? ArmCm { get; set; }

    public decimal? HipCm { get; set; }

    public string? StrengthNotes { get; set; }

    public string? Notes { get; set; }
}
