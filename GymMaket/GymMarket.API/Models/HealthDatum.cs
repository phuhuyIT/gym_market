using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class HealthDatum
{
    public string HealthDataId { get; set; } = null!;

    public string? StudentId { get; set; }

    public string? CourseId { get; set; }

    public string? BodyImage { get; set; }

    public string? ProgressNotes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<HealthIndicator> HealthIndicators { get; set; } = new List<HealthIndicator>();

    public virtual Student? Student { get; set; }
}
