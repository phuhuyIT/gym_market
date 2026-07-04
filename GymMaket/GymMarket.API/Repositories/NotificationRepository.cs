using GymMarket.API.Data;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.Hubs;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;

namespace GymMarket.API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly GymMarketContext _context;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(
            GymMarketContext context,
            IHubContext<NotificationHub> hub,
            IEmailSender emailSender,
            IConfiguration configuration,
            ILogger<NotificationRepository> logger)
        {
            _context = context;
            _hub = hub;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task NotifyUser(string userId, string type, string title, string? content = null, string? link = null)
        {
            await NotifyUsers([userId], type, title, content, link);
        }

        public async Task NotifyUsers(IEnumerable<string> userIds, string type, string title, string? content = null, string? link = null)
        {
            var normalizedType = NotificationTypes.Normalize(type);
            var targetUserIds = userIds
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (targetUserIds.Count == 0)
            {
                return;
            }

            var disabledInAppUserIds = await _context.NotificationPreferences
                .AsNoTracking()
                .Where(p => targetUserIds.Contains(p.UserId)
                    && p.Type == normalizedType
                    && !p.InAppEnabled)
                .Select(p => p.UserId)
                .ToListAsync();
            var emailPreferences = await _context.NotificationPreferences
                .AsNoTracking()
                .Where(p => targetUserIds.Contains(p.UserId)
                    && p.Type == normalizedType)
                .Select(p => new
                {
                    p.UserId,
                    p.EmailEnabled,
                    p.EmailFrequency,
                })
                .ToListAsync();
            var disabledEmailUserIds = emailPreferences
                .Where(p => !p.EmailEnabled || p.EmailFrequency == NotificationEmailFrequencies.Off)
                .Select(p => p.UserId)
                .ToList();
            var digestEmailPreferences = emailPreferences
                .Where(p => p.EmailEnabled
                    && (p.EmailFrequency == NotificationEmailFrequencies.Daily
                        || p.EmailFrequency == NotificationEmailFrequencies.Weekly))
                .ToList();
            var deliveryLogs = new List<NotificationDeliveryLog>();

            var notifications = targetUserIds
                .Except(disabledInAppUserIds)
                .Select(userId => new Notification
                {
                    UserId = userId,
                    Type = normalizedType,
                    Title = title,
                    Content = content,
                    Link = link,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                })
                .ToList();

            if (notifications.Count > 0)
            {
                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();

                foreach (var notification in notifications)
                {
                    await _hub.Clients.User(notification.UserId)
                        .SendAsync("ReceiveNotification", ToDto(notification));
                    deliveryLogs.Add(CreateDeliveryLog(
                        notification.UserId,
                        normalizedType,
                        NotificationDeliveryChannels.InApp,
                        NotificationDeliveryStatuses.Sent,
                        notification.Title,
                        notification.Content,
                        notification.Link,
                        notification.Id));
                }
            }

            foreach (var userId in targetUserIds.Intersect(disabledInAppUserIds))
            {
                deliveryLogs.Add(CreateDeliveryLog(
                    userId,
                    normalizedType,
                    NotificationDeliveryChannels.InApp,
                    NotificationDeliveryStatuses.Skipped,
                    title,
                    content,
                    link,
                    errorMessage: "Skipped by in-app notification preference."));
            }

            foreach (var userId in targetUserIds.Intersect(disabledEmailUserIds))
            {
                deliveryLogs.Add(CreateDeliveryLog(
                    userId,
                    normalizedType,
                    NotificationDeliveryChannels.Email,
                    NotificationDeliveryStatuses.Skipped,
                    title,
                    content,
                    link,
                    errorMessage: "Skipped by email notification preference."));
            }

            foreach (var preference in digestEmailPreferences)
            {
                deliveryLogs.Add(CreateDeliveryLog(
                    preference.UserId,
                    normalizedType,
                    NotificationDeliveryChannels.Email,
                    NotificationDeliveryStatuses.Deferred,
                    title,
                    content,
                    link,
                    errorMessage: $"Deferred for {preference.EmailFrequency} email digest."));
            }

            var digestEmailUserIds = digestEmailPreferences.Select(p => p.UserId).ToList();
            var emailUserIds = targetUserIds
                .Except(disabledEmailUserIds)
                .Except(digestEmailUserIds)
                .ToList();
            await SendNotificationEmails(emailUserIds, normalizedType, title, content, link, deliveryLogs);

            if (deliveryLogs.Count > 0)
            {
                _context.NotificationDeliveryLogs.AddRange(deliveryLogs);
                await _context.SaveChangesAsync();
            }
        }

        public async Task NotifyUserUpsert(string userId, string type, string title, string? content = null, string? link = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var normalizedType = NotificationTypes.Normalize(type);
            if (!await IsInAppEnabled(userId, normalizedType))
            {
                await NotifyUser(userId, normalizedType, title, content, link);
                return;
            }

            var existing = await _context.Notifications
                .Where(n => n.UserId == userId && n.Type == normalizedType && n.Link == link && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await NotifyUser(userId, normalizedType, title, content, link);
                return;
            }

            existing.Title = title;
            existing.Content = content;
            existing.CreatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Same id as before — the client replaces the entry instead of appending.
            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", ToDto(existing));
        }

        public async Task<List<NotificationDto>> GetNotificationsOfUser(
            string userId,
            int take = 50,
            int skip = 0,
            string? type = null,
            bool? isRead = null)
        {
            var safeTake = Math.Clamp(take, 1, 100);
            var safeSkip = Math.Max(skip, 0);
            var normalizedType = type?.Trim();

            var query = _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (!string.IsNullOrWhiteSpace(normalizedType))
            {
                query = query.Where(n => n.Type == normalizedType);
            }

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip(safeSkip)
                .Take(safeTake)
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
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (notifications.Count == 0)
            {
                return;
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task MarkTypeAsRead(string userId, string type)
        {
            var normalizedType = NotificationTypes.Normalize(type);
            if (string.IsNullOrWhiteSpace(normalizedType))
            {
                return;
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.Type == normalizedType && !n.IsRead)
                .ToListAsync();

            if (notifications.Count == 0)
            {
                return;
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationPreferenceDto>> GetPreferences(string userId)
        {
            var stored = await _context.NotificationPreferences
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return NotificationTypes.All
                .Select(type =>
                {
                    var preference = stored.FirstOrDefault(p => p.Type == type);
                    return new NotificationPreferenceDto
                    {
                        Type = type,
                        Label = NotificationTypes.LabelFor(type),
                        InAppEnabled = preference?.InAppEnabled ?? true,
                        EmailEnabled = ResolveEmailEnabled(preference),
                        EmailFrequency = ResolveEmailFrequency(preference),
                    };
                })
                .ToList();
        }

        public async Task<List<NotificationPreferenceDto>> UpdatePreferences(
            string userId,
            IEnumerable<NotificationPreferenceUpdateDto> preferences)
        {
            var requested = preferences
                .Where(p => NotificationTypes.IsSupported(p.Type))
                .Select(p => new
                {
                    Type = NotificationTypes.Normalize(p.Type),
                    p.InAppEnabled,
                    p.EmailEnabled,
                    p.EmailFrequency,
                })
                .GroupBy(p => p.Type)
                .Select(g => g.Last())
                .ToList();

            if (requested.Count == 0)
            {
                return await GetPreferences(userId);
            }

            var requestedTypes = requested.Select(p => p.Type).ToList();
            var existing = await _context.NotificationPreferences
                .Where(p => p.UserId == userId && requestedTypes.Contains(p.Type))
                .ToListAsync();
            var now = DateTime.UtcNow;

            foreach (var item in requested)
            {
                var preference = existing.FirstOrDefault(p => p.Type == item.Type);
                var emailFrequency = ResolveRequestedEmailFrequency(item.EmailEnabled, item.EmailFrequency, preference);
                var emailEnabled = ResolveRequestedEmailEnabled(item.EmailEnabled, emailFrequency, preference);

                if (preference == null)
                {
                    _context.NotificationPreferences.Add(new NotificationPreference
                    {
                        UserId = userId,
                        Type = item.Type,
                        InAppEnabled = item.InAppEnabled,
                        EmailEnabled = emailEnabled,
                        EmailFrequency = emailFrequency,
                        CreatedAt = now,
                        UpdatedAt = now,
                    });
                }
                else
                {
                    preference.InAppEnabled = item.InAppEnabled;
                    preference.EmailEnabled = emailEnabled;
                    preference.EmailFrequency = emailFrequency;
                    preference.UpdatedAt = now;
                }
            }

            await _context.SaveChangesAsync();
            return await GetPreferences(userId);
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

        private async Task<bool> IsInAppEnabled(string userId, string type)
        {
            var preference = await _context.NotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type);

            return preference?.InAppEnabled ?? true;
        }

        private static bool ResolveEmailEnabled(NotificationPreference? preference)
        {
            return preference == null
                || (preference.EmailEnabled && ResolveEmailFrequency(preference) != NotificationEmailFrequencies.Off);
        }

        private static string ResolveEmailFrequency(NotificationPreference? preference)
        {
            if (preference == null)
            {
                return NotificationEmailFrequencies.Immediate;
            }

            if (!preference.EmailEnabled)
            {
                return NotificationEmailFrequencies.Off;
            }

            return NotificationEmailFrequencies.IsSupported(preference.EmailFrequency)
                ? NotificationEmailFrequencies.Normalize(preference.EmailFrequency)
                : NotificationEmailFrequencies.Immediate;
        }

        private static string ResolveRequestedEmailFrequency(
            bool? requestedEmailEnabled,
            string? requestedFrequency,
            NotificationPreference? existing)
        {
            if (requestedEmailEnabled == false)
            {
                return NotificationEmailFrequencies.Off;
            }

            if (!string.IsNullOrWhiteSpace(requestedFrequency)
                && NotificationEmailFrequencies.IsSupported(requestedFrequency))
            {
                return NotificationEmailFrequencies.Normalize(requestedFrequency);
            }

            if (requestedEmailEnabled == true && ResolveEmailFrequency(existing) == NotificationEmailFrequencies.Off)
            {
                return NotificationEmailFrequencies.Immediate;
            }

            return ResolveEmailFrequency(existing);
        }

        private static bool ResolveRequestedEmailEnabled(
            bool? requestedEmailEnabled,
            string emailFrequency,
            NotificationPreference? existing)
        {
            if (emailFrequency == NotificationEmailFrequencies.Off)
            {
                return false;
            }

            return requestedEmailEnabled ?? existing?.EmailEnabled ?? true;
        }

        private async Task SendNotificationEmails(
            IReadOnlyCollection<string> userIds,
            string type,
            string title,
            string? content,
            string? link,
            ICollection<NotificationDeliveryLog> deliveryLogs)
        {
            if (userIds.Count == 0)
            {
                return;
            }

            var users = await _context.Users
                .AsNoTracking()
                .Where(user => userIds.Contains(user.Id)
                    && user.Email != null
                    && user.Email != string.Empty)
                .Select(user => new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                })
                .ToListAsync();
            var usersById = users.ToDictionary(user => user.Id);
            var missingEmailUserIds = userIds.Except(usersById.Keys).ToList();
            foreach (var userId in missingEmailUserIds)
            {
                deliveryLogs.Add(CreateDeliveryLog(
                    userId,
                    type,
                    NotificationDeliveryChannels.Email,
                    NotificationDeliveryStatuses.Skipped,
                    title,
                    content,
                    link,
                    errorMessage: "Skipped because the user has no email address."));
            }

            var template = await GetActiveTemplate(type);

            foreach (var user in users)
            {
                var recipientName = user.FullName ?? user.Email!;
                var actionUrl = BuildAbsoluteUrl(link);
                var variables = BuildTemplateVariables(
                    recipientName,
                    user.Email!,
                    NotificationTypes.LabelFor(type),
                    title,
                    content,
                    link,
                    actionUrl);
                var subject = template == null
                    ? title
                    : TrimToLength(RenderTemplate(template.SubjectTemplate, variables, htmlEncodeValues: false), 250) ?? string.Empty;
                var body = template == null
                    ? BuildEmailBody(recipientName, NotificationTypes.LabelFor(type), title, content, link)
                    : TrimToLength(RenderTemplate(template.BodyTemplate, variables, htmlEncodeValues: true), 4000) ?? string.Empty;

                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email!,
                        subject,
                        body);
                    deliveryLogs.Add(CreateDeliveryLog(
                        user.Id,
                        type,
                        NotificationDeliveryChannels.Email,
                        NotificationDeliveryStatuses.Sent,
                        subject,
                        body,
                        link,
                        recipientEmail: user.Email,
                        recipientName: user.FullName));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send notification email {NotificationType} to user {UserId}",
                        type,
                        user.Id);
                    deliveryLogs.Add(CreateDeliveryLog(
                        user.Id,
                        type,
                        NotificationDeliveryChannels.Email,
                        NotificationDeliveryStatuses.Failed,
                        subject,
                        body,
                        link,
                        recipientEmail: user.Email,
                        recipientName: user.FullName,
                        errorMessage: TrimToLength(ex.Message, 1000)));
                }
            }
        }

        private async Task<NotificationTemplate?> GetActiveTemplate(string type)
        {
            var template = await _context.NotificationTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Type == type);

            return template?.IsActive == true ? template : null;
        }

        private string BuildEmailBody(string recipientName, string label, string title, string? content, string? link)
        {
            var encodedName = WebUtility.HtmlEncode(recipientName);
            var encodedLabel = WebUtility.HtmlEncode(label);
            var encodedTitle = WebUtility.HtmlEncode(title);
            var encodedContent = WebUtility.HtmlEncode(content ?? "Open GymMarket to view the latest update.");
            var actionUrl = BuildAbsoluteUrl(link);
            var action = string.IsNullOrWhiteSpace(actionUrl)
                ? string.Empty
                : $"""<p><a href="{WebUtility.HtmlEncode(actionUrl)}">Open in GymMarket</a></p>""";

            return $"""
                <p>Hello {encodedName},</p>
                <p><strong>{encodedTitle}</strong></p>
                <p>{encodedContent}</p>
                <p>Category: {encodedLabel}</p>
                {action}
                <p>You can change email delivery from your notification preferences.</p>
                """;
        }

        private string? BuildAbsoluteUrl(string? link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return null;
            }

            if (Uri.TryCreate(link, UriKind.Absolute, out var absoluteUri)
                && absoluteUri.IsAbsoluteUri
                && (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps))
            {
                return link;
            }

            var clientBaseUrl = _configuration["App:ClientBaseUrl"]?.TrimEnd('/');
            clientBaseUrl = string.IsNullOrWhiteSpace(clientBaseUrl) ? "http://localhost:4200" : clientBaseUrl;

            return $"{clientBaseUrl}/{link.TrimStart('/')}";
        }

        private static Dictionary<string, string> BuildTemplateVariables(
            string userName,
            string userEmail,
            string category,
            string title,
            string? content,
            string? link,
            string? actionUrl)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["userName"] = userName,
                ["userEmail"] = userEmail,
                ["category"] = category,
                ["title"] = title,
                ["content"] = content ?? "Open GymMarket to view the latest update.",
                ["link"] = link ?? string.Empty,
                ["actionUrl"] = actionUrl ?? string.Empty,
            };
        }

        private static string RenderTemplate(
            string template,
            IReadOnlyDictionary<string, string> variables,
            bool htmlEncodeValues)
        {
            return Regex.Replace(template, @"\{\{\s*([A-Za-z0-9_]+)\s*\}\}", match =>
            {
                var key = match.Groups[1].Value;
                if (!variables.TryGetValue(key, out var value))
                {
                    return match.Value;
                }

                return htmlEncodeValues ? WebUtility.HtmlEncode(value) : value;
            });
        }

        private static NotificationDeliveryLog CreateDeliveryLog(
            string userId,
            string type,
            string channel,
            string status,
            string subject,
            string? content,
            string? link,
            int? notificationId = null,
            string? recipientEmail = null,
            string? recipientName = null,
            string? errorMessage = null)
        {
            return new NotificationDeliveryLog
            {
                UserId = userId,
                Type = type,
                Channel = channel,
                Status = status,
                Subject = TrimToLength(subject, 250) ?? string.Empty,
                Content = TrimToLength(content, 4000),
                Link = TrimToLength(link, 1000),
                NotificationId = notificationId,
                RecipientEmail = TrimToLength(recipientEmail, 256),
                RecipientName = TrimToLength(recipientName, 450),
                ErrorMessage = TrimToLength(errorMessage, 1000),
                CreatedAt = DateTime.UtcNow,
            };
        }

        private static string? TrimToLength(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value[..maxLength];
        }
    }
}
