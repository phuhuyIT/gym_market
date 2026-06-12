using System;

namespace GymMarket.API.Models;

// General-purpose per-user notification. Any feature can create one through
// INotificationRepository; it is persisted and pushed in real time over the
// notification hub to the targeted user.
public partial class Notification
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Type { get; set; } = NotificationTypes.System;

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    // Optional in-app route the client navigates to when the notification is clicked.
    public string? Link { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? User { get; set; }
}
