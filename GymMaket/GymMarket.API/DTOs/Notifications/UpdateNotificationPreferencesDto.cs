namespace GymMarket.API.DTOs.Notifications;

public class UpdateNotificationPreferencesDto
{
    public List<NotificationPreferenceUpdateDto> Preferences { get; set; } = [];
}
