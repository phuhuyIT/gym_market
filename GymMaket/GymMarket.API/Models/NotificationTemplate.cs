namespace GymMarket.API.Models;

public partial class NotificationTemplate
{
    public int Id { get; set; }

    public string Type { get; set; } = NotificationTypes.System;

    public string SubjectTemplate { get; set; } = string.Empty;

    public string BodyTemplate { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? UpdatedById { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
