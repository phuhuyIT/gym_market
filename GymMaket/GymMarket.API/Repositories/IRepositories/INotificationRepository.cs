using GymMarket.API.DTOs.Notifications;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface INotificationRepository
    {
        // Multi-purpose entry point: any feature can notify a user. Persists the
        // notification and pushes it in real time over the notification hub.
        Task NotifyUser(string userId, string type, string title, string? content = null, string? link = null);
        Task NotifyUsers(IEnumerable<string> userIds, string type, string title, string? content = null, string? link = null);

        // Collapsing variant for high-frequency events (e.g. chat messages): if the
        // user already has an UNREAD notification with the same type + link, it is
        // refreshed in place instead of stacking a new row per event.
        Task NotifyUserUpsert(string userId, string type, string title, string? content = null, string? link = null);

        Task<List<NotificationDto>> GetNotificationsOfUser(
            string userId,
            int take = 50,
            int skip = 0,
            string? type = null,
            bool? isRead = null);
        Task<int> GetUnreadCount(string userId);
        Task MarkAsRead(int id, string userId);
        Task MarkAllAsRead(string userId);
        Task MarkTypeAsRead(string userId, string type);
        Task<List<NotificationPreferenceDto>> GetPreferences(string userId);
        Task<List<NotificationPreferenceDto>> UpdatePreferences(
            string userId,
            IEnumerable<NotificationPreferenceUpdateDto> preferences);
    }
}
