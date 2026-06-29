namespace GymMarket.API.DTOs.Notifications;

public class NotificationPreferenceDto
{
    public string Type { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public bool InAppEnabled { get; set; }

    public bool EmailEnabled { get; set; }
}
