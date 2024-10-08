using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class HealthIndicator
{
    public string IndicatorId { get; set; } = null!;

    public string? HealthDataId { get; set; }

    public string? IndicatorName { get; set; }

    public string? IndicatorValue { get; set; }

    public DateTime? MeasurementDate { get; set; }

    public virtual HealthDatum? HealthData { get; set; }
}
