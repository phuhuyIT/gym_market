using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseLiveSessions;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseLiveSessionsController : ControllerBase
{
    private readonly GymMarketContext _context;
    private readonly ICourseAccessService _courseAccessService;
    private readonly INotificationRepository _notificationRepository;

    public CourseLiveSessionsController(
        GymMarketContext context,
        ICourseAccessService courseAccessService,
        INotificationRepository notificationRepository)
    {
        _context = context;
        _courseAccessService = courseAccessService;
        _notificationRepository = notificationRepository;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseSessions(string courseId)
    {
        var canManage = await _courseAccessService.CanManageCourseAsync(User, courseId);
        if (!canManage && !await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var query = _context.CourseLiveSessions
            .AsNoTracking()
            .Where(s => s.CourseId == courseId);

        if (!canManage)
            query = query.Where(s => s.Status != CourseLiveSessionStatus.Draft);

        var sessions = await query
            .OrderBy(s => s.StartsAt)
            .ToListAsync();

        return Ok(sessions.Select(ToDto));
    }

    [HttpGet("course/{courseId}/manage")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetCourseSessionsForManagement(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var sessions = await _context.CourseLiveSessions
            .AsNoTracking()
            .Where(s => s.CourseId == courseId)
            .OrderByDescending(s => s.StartsAt)
            .ToListAsync();

        return Ok(sessions.Select(ToDto));
    }

    [HttpPost("course/{courseId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> CreateSession(string courseId, UpsertCourseLiveSessionDto dto)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var validationError = Validate(dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var now = DateTime.UtcNow;
        var session = new CourseLiveSession
        {
            LiveSessionId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            CreatedAt = now,
        };

        Apply(session, dto, now);
        _context.CourseLiveSessions.Add(session);
        await _context.SaveChangesAsync();

        if (session.Status == CourseLiveSessionStatus.Scheduled)
            await NotifyLearnersAsync(session, "Live session scheduled");

        return Ok(ToDto(session));
    }

    [HttpPut("{liveSessionId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> UpdateSession(string liveSessionId, UpsertCourseLiveSessionDto dto)
    {
        var session = await _context.CourseLiveSessions.FirstOrDefaultAsync(s => s.LiveSessionId == liveSessionId);
        if (session == null)
            return NotFound(new { Message = "LIVE_SESSION_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, session.CourseId))
            return Forbid();

        var validationError = Validate(dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var wasLearnerVisible = CourseLiveSessionStatus.IsLearnerVisible(session.Status);
        Apply(session, dto, DateTime.UtcNow);
        await _context.SaveChangesAsync();

        if (!wasLearnerVisible && session.Status == CourseLiveSessionStatus.Scheduled)
            await NotifyLearnersAsync(session, "Live session scheduled");

        return Ok(ToDto(session));
    }

    [HttpDelete("{liveSessionId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> DeleteSession(string liveSessionId)
    {
        var session = await _context.CourseLiveSessions.FirstOrDefaultAsync(s => s.LiveSessionId == liveSessionId);
        if (session == null)
            return NotFound(new { Message = "LIVE_SESSION_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, session.CourseId))
            return Forbid();

        _context.CourseLiveSessions.Remove(session);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static string? Validate(UpsertCourseLiveSessionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return "LIVE_SESSION_TITLE_REQUIRED";

        if (dto.StartsAt == default || dto.EndsAt == default || dto.EndsAt <= dto.StartsAt)
            return "LIVE_SESSION_TIME_INVALID";

        if (!CourseLiveSessionStatus.All.Any(status => string.Equals(status, dto.Status, StringComparison.OrdinalIgnoreCase)))
            return "LIVE_SESSION_STATUS_INVALID";

        if (!IsValidUrl(dto.MeetingUrl))
            return "LIVE_SESSION_MEETING_URL_INVALID";

        if (!IsValidUrl(dto.RecordingUrl))
            return "LIVE_SESSION_RECORDING_URL_INVALID";

        return null;
    }

    private static bool IsValidUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            || Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static void Apply(CourseLiveSession session, UpsertCourseLiveSessionDto dto, DateTime now)
    {
        var status = CourseLiveSessionStatus.Normalize(dto.Status);
        session.Title = dto.Title.Trim();
        session.Description = dto.Description?.Trim();
        session.StartsAt = dto.StartsAt.ToUniversalTime();
        session.EndsAt = dto.EndsAt.ToUniversalTime();
        session.MeetingUrl = dto.MeetingUrl?.Trim();
        session.RecordingUrl = dto.RecordingUrl?.Trim();
        session.Status = status;
        session.AttendanceRequired = dto.AttendanceRequired;
        session.UpdatedAt = now;
        session.PublishedAt = CourseLiveSessionStatus.IsLearnerVisible(status)
            ? session.PublishedAt ?? now
            : null;
    }

    private async Task NotifyLearnersAsync(CourseLiveSession session, string title)
    {
        var targetUserIds = await GetLearnerUserIdsAsync(session.CourseId);
        if (targetUserIds.Count == 0)
            return;

        await _notificationRepository.NotifyUsers(
            targetUserIds,
            NotificationTypes.LiveSession,
            title,
            $"{session.Title} starts at {session.StartsAt:yyyy-MM-dd HH:mm} UTC.",
            $"/client/course-live-sessions/{session.CourseId}");
    }

    private async Task<List<string>> GetLearnerUserIdsAsync(string courseId)
    {
        var paidStatuses = new[] { PaymentStatus.Paid, PaymentStatus.Completed };
        var paymentUserIds = await _context.Payments
            .AsNoTracking()
            .Where(p => p.CourseId == courseId && paidStatuses.Contains(p.PaymentStatus!))
            .Select(p => p.Student!.UserId)
            .ToListAsync();

        var registrationUserIds = await _context.CourseRegistrations
            .AsNoTracking()
            .Where(r => r.CourseId == courseId && paidStatuses.Contains(r.PaymentStatus!))
            .Select(r => r.Student!.UserId)
            .ToListAsync();

        return paymentUserIds
            .Concat(registrationUserIds)
            .Where(userId => !string.IsNullOrWhiteSpace(userId))
            .Distinct()
            .Cast<string>()
            .ToList();
    }

    private static CourseLiveSessionDto ToDto(CourseLiveSession session) => new()
    {
        LiveSessionId = session.LiveSessionId,
        CourseId = session.CourseId,
        Title = session.Title,
        Description = session.Description,
        StartsAt = session.StartsAt,
        EndsAt = session.EndsAt,
        MeetingUrl = session.MeetingUrl,
        RecordingUrl = session.RecordingUrl,
        Status = session.Status,
        AttendanceRequired = session.AttendanceRequired,
        CreatedAt = session.CreatedAt,
        UpdatedAt = session.UpdatedAt,
        PublishedAt = session.PublishedAt,
    };
}
