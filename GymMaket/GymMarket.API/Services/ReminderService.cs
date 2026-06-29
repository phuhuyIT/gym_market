using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Services;

public class ReminderService : IReminderService
{
    private static readonly TimeSpan DeduplicationWindow = TimeSpan.FromHours(20);

    private readonly GymMarketContext _context;
    private readonly INotificationRepository _notificationRepository;

    public ReminderService(GymMarketContext context, INotificationRepository notificationRepository)
    {
        _context = context;
        _notificationRepository = notificationRepository;
    }

    public async Task<int> SendDueRemindersAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var sent = 0;

        sent += await SendUpcomingClassReminders(now, cancellationToken);
        sent += await SendMembershipExpiryReminders(now, cancellationToken);
        sent += await SendProgressCheckInReminders(now, cancellationToken);
        sent += await SendWorkoutInactivityReminders(now, cancellationToken);

        return sent;
    }

    private async Task<int> SendUpcomingClassReminders(DateTime now, CancellationToken cancellationToken)
    {
        var cutoff = now.AddHours(24);
        var bookings = await _context.ClassBookings
            .AsNoTracking()
            .Include(b => b.Student)
            .Include(b => b.ClassSession)
                .ThenInclude(s => s!.Trainer)
            .Where(b => b.Status == ClassBookingStatus.Booked
                && b.Student != null
                && b.Student.UserId != null
                && b.ClassSession != null
                && b.ClassSession.Status == ClassSessionStatus.Scheduled
                && b.ClassSession.StartsAt > now
                && b.ClassSession.StartsAt <= cutoff)
            .ToListAsync(cancellationToken);

        var sent = 0;
        foreach (var booking in bookings)
        {
            var session = booking.ClassSession!;
            var content = $"{session.Title} starts on {FormatDateTime(session.StartsAt)}.";
            sent += await NotifyOnce(
                booking.Student!.UserId!,
                NotificationTypes.Class,
                "Class starts soon",
                content,
                "/client/classes",
                now,
                cancellationToken);
        }

        var trainerSessions = bookings
            .Where(b => !string.IsNullOrWhiteSpace(b.ClassSession?.Trainer?.UserId))
            .GroupBy(b => b.ClassSession!.ClassSessionId)
            .Select(g => new
            {
                Session = g.First().ClassSession!,
                BookingCount = g.Count()
            })
            .ToList();

        foreach (var item in trainerSessions)
        {
            var session = item.Session;
            var content = $"{session.Title} starts on {FormatDateTime(session.StartsAt)} with {item.BookingCount} booking(s).";
            sent += await NotifyOnce(
                session.Trainer!.UserId!,
                NotificationTypes.Class,
                "Class starts soon",
                content,
                "/agency/classes",
                now,
                cancellationToken);
        }

        return sent;
    }

    private async Task<int> SendMembershipExpiryReminders(DateTime now, CancellationToken cancellationToken)
    {
        var cutoff = now.AddDays(7);
        var memberships = await _context.StudentMemberships
            .AsNoTracking()
            .Include(m => m.Student)
            .Include(m => m.Plan)
            .Where(m => m.Status == MembershipStatus.Active
                && m.StartsAt <= now
                && m.EndsAt >= now
                && m.EndsAt <= cutoff
                && m.Student != null
                && m.Student.UserId != null)
            .ToListAsync(cancellationToken);

        var sent = 0;
        foreach (var membership in memberships)
        {
            var daysLeft = Math.Max(0, (membership.EndsAt.Date - now.Date).Days);
            var expiryCopy = daysLeft == 0
                ? "expires today"
                : $"expires in {daysLeft} day{(daysLeft == 1 ? string.Empty : "s")}";
            var planName = membership.Plan?.Name ?? "Your membership";

            sent += await NotifyOnce(
                membership.Student!.UserId!,
                NotificationTypes.Membership,
                "Membership expiring soon",
                $"{planName} {expiryCopy}. Renew to keep booking classes.",
                "/client/membership",
                now,
                cancellationToken);
        }

        return sent;
    }

    private async Task<int> SendProgressCheckInReminders(DateTime now, CancellationToken cancellationToken)
    {
        var staleBefore = now.AddDays(-7);
        var activeStudents = await _context.StudentWorkoutAssignments
            .AsNoTracking()
            .Include(a => a.Student)
            .Where(a => a.Status == WorkoutAssignmentStatus.Active
                && a.CreatedAt <= staleBefore
                && a.Student != null
                && a.Student.UserId != null)
            .Select(a => new
            {
                a.StudentId,
                StudentUserId = a.Student!.UserId!
            })
            .Distinct()
            .ToListAsync(cancellationToken);

        var sent = 0;
        foreach (var student in activeStudents)
        {
            var latestLogAt = await _context.StudentProgressLogs
                .AsNoTracking()
                .Where(l => l.StudentId == student.StudentId)
                .OrderByDescending(l => l.LoggedAt)
                .Select(l => (DateTime?)l.LoggedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestLogAt.HasValue && latestLogAt.Value > staleBefore)
            {
                continue;
            }

            sent += await NotifyOnce(
                student.StudentUserId,
                NotificationTypes.Progress,
                "Progress check-in due",
                "Add a progress log so your trainer can review your latest check-in.",
                "/client/progress",
                now,
                cancellationToken);
        }

        return sent;
    }

    private async Task<int> SendWorkoutInactivityReminders(DateTime now, CancellationToken cancellationToken)
    {
        var staleBefore = now.AddDays(-7);
        var assignments = await _context.StudentWorkoutAssignments
            .AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.WorkoutPlan)
            .Include(a => a.Completions)
            .Where(a => a.Status == WorkoutAssignmentStatus.Active
                && a.CreatedAt <= staleBefore
                && a.Student != null
                && a.Student.UserId != null)
            .ToListAsync(cancellationToken);

        var sent = 0;
        foreach (var assignment in assignments)
        {
            var lastCompletedAt = assignment.Completions
                .OrderByDescending(c => c.CompletedAt)
                .Select(c => (DateTime?)c.CompletedAt)
                .FirstOrDefault();

            if (lastCompletedAt.HasValue && lastCompletedAt.Value > staleBefore)
            {
                continue;
            }

            sent += await NotifyOnce(
                assignment.Student!.UserId!,
                NotificationTypes.Workout,
                "Workout waiting",
                $"{assignment.WorkoutPlan?.Name ?? "Your workout plan"} has not had activity this week.",
                "/client/workouts",
                now,
                cancellationToken);
        }

        return sent;
    }

    private async Task<int> NotifyOnce(
        string userId,
        string type,
        string title,
        string content,
        string link,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var since = now.Subtract(DeduplicationWindow);
        var alreadySent = await _context.Notifications
            .AsNoTracking()
            .AnyAsync(n => n.UserId == userId
                && n.Type == type
                && n.Title == title
                && n.Content == content
                && n.Link == link
                && n.CreatedAt >= since,
                cancellationToken);

        if (alreadySent)
        {
            return 0;
        }

        await _notificationRepository.NotifyUser(userId, type, title, content, link);
        return 1;
    }

    private static string FormatDateTime(DateTime dateTime)
    {
        return $"{dateTime:MMM d, yyyy h:mm tt} UTC";
    }
}
