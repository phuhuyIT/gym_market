using System.Net;
using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Services;

public class NotificationDigestService : INotificationDigestService
{
    private readonly GymMarketContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationDigestService> _logger;

    public NotificationDigestService(
        GymMarketContext context,
        IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<NotificationDigestService> logger)
    {
        _context = context;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> SendDueDigestsAsync(DateTime? utcNow = null, CancellationToken cancellationToken = default)
    {
        var now = utcNow ?? DateTime.UtcNow;
        var dailyCutoff = now.AddDays(-1);
        var weeklyCutoff = now.AddDays(-7);

        var candidates = await _context.NotificationDeliveryLogs
            .Where(log => log.Channel == NotificationDeliveryChannels.Email
                && log.Status == NotificationDeliveryStatuses.Deferred
                && log.CreatedAt <= dailyCutoff)
            .OrderBy(log => log.CreatedAt)
            .ToListAsync(cancellationToken);

        if (candidates.Count == 0)
        {
            return 0;
        }

        var userIds = candidates.Select(log => log.UserId).Distinct().ToList();
        var types = candidates.Select(log => log.Type).Distinct().ToList();
        var preferences = await _context.NotificationPreferences
            .AsNoTracking()
            .Where(preference => userIds.Contains(preference.UserId) && types.Contains(preference.Type))
            .Select(preference => new
            {
                preference.UserId,
                preference.Type,
                preference.EmailEnabled,
                preference.EmailFrequency,
            })
            .ToListAsync(cancellationToken);

        var preferenceLookup = preferences.ToDictionary(
            preference => (preference.UserId, preference.Type),
            preference => preference);
        var due = new List<(NotificationDeliveryLog Log, string Frequency)>();

        foreach (var log in candidates)
        {
            if (!preferenceLookup.TryGetValue((log.UserId, log.Type), out var preference))
            {
                MarkSkipped(log, "Digest skipped because the preference no longer uses digest delivery.");
                continue;
            }

            if (!preference.EmailEnabled || preference.EmailFrequency == NotificationEmailFrequencies.Off)
            {
                MarkSkipped(log, "Digest skipped by current email notification preference.");
                continue;
            }

            if (preference.EmailFrequency == NotificationEmailFrequencies.Daily)
            {
                due.Add((log, NotificationEmailFrequencies.Daily));
            }
            else if (preference.EmailFrequency == NotificationEmailFrequencies.Weekly && log.CreatedAt <= weeklyCutoff)
            {
                due.Add((log, NotificationEmailFrequencies.Weekly));
            }
            else if (preference.EmailFrequency == NotificationEmailFrequencies.Immediate)
            {
                MarkSkipped(log, "Digest skipped because the preference now sends immediate emails.");
            }
        }

        if (due.Count == 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return 0;
        }

        var dueUserIds = due.Select(item => item.Log.UserId).Distinct().ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(user => dueUserIds.Contains(user.Id))
            .Select(user => new
            {
                user.Id,
                user.Email,
                user.FullName,
            })
            .ToListAsync(cancellationToken);
        var usersById = users.ToDictionary(user => user.Id);
        var sentCount = 0;

        foreach (var group in due.GroupBy(item => new { item.Log.UserId, item.Frequency }))
        {
            if (!usersById.TryGetValue(group.Key.UserId, out var user) || string.IsNullOrWhiteSpace(user.Email))
            {
                foreach (var item in group)
                {
                    MarkSkipped(item.Log, "Digest skipped because the user has no email address.");
                }
                continue;
            }

            var logs = group.Select(item => item.Log).OrderBy(log => log.CreatedAt).ToList();
            var recipientName = user.FullName ?? user.Email;
            var subject = $"Your GymMarket {group.Key.Frequency} digest";
            var body = BuildDigestBody(recipientName, group.Key.Frequency, logs);

            try
            {
                await _emailSender.SendEmailAsync(user.Email!, subject, body);
                sentCount++;

                foreach (var log in logs)
                {
                    log.Status = NotificationDeliveryStatuses.Digested;
                    log.RecipientEmail = user.Email;
                    log.RecipientName = user.FullName;
                    log.ErrorMessage = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send {Frequency} notification digest to user {UserId}",
                    group.Key.Frequency,
                    group.Key.UserId);

                foreach (var log in logs)
                {
                    log.ErrorMessage = TrimToLength(ex.Message, 1000);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return sentCount;
    }

    private string BuildDigestBody(string recipientName, string frequency, IReadOnlyList<NotificationDeliveryLog> logs)
    {
        var encodedName = WebUtility.HtmlEncode(recipientName);
        var encodedFrequency = WebUtility.HtmlEncode(frequency);
        var rows = logs
            .Select(log =>
            {
                var title = WebUtility.HtmlEncode(log.Subject);
                var category = WebUtility.HtmlEncode(NotificationTypes.LabelFor(log.Type));
                var content = WebUtility.HtmlEncode(log.Content ?? "Open GymMarket to view the update.");
                var actionUrl = BuildAbsoluteUrl(log.Link);
                var action = string.IsNullOrWhiteSpace(actionUrl)
                    ? string.Empty
                    : $"""<p><a href="{WebUtility.HtmlEncode(actionUrl)}">Open update</a></p>""";

                return $"""
                    <li>
                        <p><strong>{title}</strong></p>
                        <p>{content}</p>
                        <p>Category: {category}</p>
                        {action}
                    </li>
                    """;
            });

        return $"""
            <p>Hello {encodedName},</p>
            <p>Here is your {encodedFrequency} GymMarket notification digest.</p>
            <ul>
            {string.Join(Environment.NewLine, rows)}
            </ul>
            <p>You can change digest delivery from your notification preferences.</p>
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

    private static void MarkSkipped(NotificationDeliveryLog log, string reason)
    {
        log.Status = NotificationDeliveryStatuses.Skipped;
        log.ErrorMessage = reason;
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
