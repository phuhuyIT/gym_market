using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Notification
{
    public string NotificationId { get; set; } = null!;

    public string? StudentId { get; set; }

    public string? NotificationContent { get; set; }

    public string? NotificationType { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Student? Student { get; set; }
}
