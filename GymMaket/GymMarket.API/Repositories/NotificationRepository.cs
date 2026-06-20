using GymMarket.API.Data;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.Hubs;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly GymMarketContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationRepository(GymMarketContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task NotifyUser(string userId, string type, string title, string? content = null, string? link = null)
        {
            await NotifyUsers([userId], type, title, content, link);
        }

        public async Task NotifyUsers(IEnumerable<string> userIds, string type, string title, string? content = null, string? link = null)
        {
            var notifications = userIds
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .Select(userId => new Notification
                {
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Content = content,
                    Link = link,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                })
                .ToList();

            if (notifications.Count == 0)
            {
                return;
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            foreach (var notification in notifications)
            {
                await _hub.Clients.User(notification.UserId)
                    .SendAsync("ReceiveNotification", ToDto(notification));
            }
        }

        public async Task NotifyUserUpsert(string userId, string type, string title, string? content = null, string? link = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var existing = await _context.Notifications
                .Where(n => n.UserId == userId && n.Type == type && n.Link == link && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await NotifyUser(userId, type, title, content, link);
                return;
            }

            existing.Title = title;
            existing.Content = content;
            existing.CreatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Same id as before — the client replaces the entry instead of appending.
            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", ToDto(existing));
        }

        public async Task<List<NotificationDto>> GetNotificationsOfUser(string userId, int take = 50)
        {
            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Type = n.Type,
                    Title = n.Title,
                    Content = n.Content,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                })
                .ToListAsync();
            // Stored as UTC, but SQL roundtrips lose the Kind; re-stamp so the JSON
            // carries 'Z' and clients convert to their local time.
            foreach (var notification in notifications)
            {
                notification.CreatedAt = DateTime.SpecifyKind(notification.CreatedAt, DateTimeKind.Utc);
            }
            return notifications;
        }

        public async Task<int> GetUnreadCount(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsRead(int id, string userId)
        {
            // userId guard so a user cannot mark someone else's notification.
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead(string userId)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        private static NotificationDto ToDto(Notification n)
        {
            return new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Content = n.Content,
                Link = n.Link,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
            };
        }
    }
}
