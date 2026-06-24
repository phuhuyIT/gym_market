using System;

namespace GymMarket.API.Models;

public partial class PaymentEvent
{
    public string PaymentEventId { get; set; } = null!;

    public string PaymentId { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string? OldStatus { get; set; }

    public string? NewStatus { get; set; }

    public string Source { get; set; } = null!;

    public string? Message { get; set; }

    public string? RawPayload { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Payment? Payment { get; set; }
}
