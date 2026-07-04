namespace GymMarket.API.DTOs.Notifications;

public class NotificationPreferenceUpdateDto
{
    public string Type { get; set; } = string.Empty;

    public bool InAppEnabled { get; set; }

    public bool? EmailEnabled { get; set; }

    public string? EmailFrequency { get; set; }
}
