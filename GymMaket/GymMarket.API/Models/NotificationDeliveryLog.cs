namespace GymMarket.API.Models;

public partial class NotificationDeliveryLog
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? RecipientEmail { get; set; }

    public string? RecipientName { get; set; }

    public string Type { get; set; } = NotificationTypes.System;

    public string Channel { get; set; } = NotificationDeliveryChannels.InApp;

    public string Status { get; set; } = NotificationDeliveryStatuses.Sent;

    public string Subject { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string? Link { get; set; }

    public int? NotificationId { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? User { get; set; }

    public virtual Notification? Notification { get; set; }
}
