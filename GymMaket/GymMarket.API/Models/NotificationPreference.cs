using System;

namespace GymMarket.API.Models;

public partial class NotificationPreference
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Type { get; set; } = NotificationTypes.System;

    public bool InAppEnabled { get; set; } = true;

    public bool EmailEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? User { get; set; }
}
