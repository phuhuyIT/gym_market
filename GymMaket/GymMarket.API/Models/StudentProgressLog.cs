namespace GymMarket.API.Models;

public class StudentProgressLog
{
    public string ProgressLogId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public DateTime LoggedAt { get; set; }

    public decimal? WeightKg { get; set; }

    public decimal? BodyFatPercent { get; set; }

    public decimal? WaistCm { get; set; }

    public decimal? ChestCm { get; set; }

    public decimal? ArmCm { get; set; }

    public decimal? HipCm { get; set; }

    public string? StrengthNotes { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual Student? Student { get; set; }
}
