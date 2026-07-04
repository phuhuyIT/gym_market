using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseCalendar;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseCalendarController : ControllerBase
{
    private readonly GymMarketContext _context;
    private readonly ICourseAccessService _courseAccessService;

    public CourseCalendarController(GymMarketContext context, ICourseAccessService courseAccessService)
    {
        _context = context;
        _courseAccessService = courseAccessService;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseCalendar(string courseId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var canManage = await _courseAccessService.CanManageCourseAsync(User, courseId);
        if (!canManage && !await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var range = BuildRange(from, to);
        if (range == null)
            return BadRequest(new { Message = "CALENDAR_RANGE_INVALID" });

        return Ok(await BuildCalendarItems([courseId], canManage, range.Value.From, range.Value.To));
    }

    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyCalendar([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var studentId = User.FindFirstValue("studentId");
        if (string.IsNullOrWhiteSpace(studentId))
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

        var range = BuildRange(from, to);
        if (range == null)
            return BadRequest(new { Message = "CALENDAR_RANGE_INVALID" });

        var courseIds = await GetPaidCourseIdsAsync(studentId);
        if (courseIds.Count == 0)
            return Ok(new List<CourseCalendarItemDto>());

        return Ok(await BuildCalendarItems(courseIds, canManage: false, range.Value.From, range.Value.To));
    }

    [HttpGet("trainer")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetTrainerCalendar([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var range = BuildRange(from, to);
        if (range == null)
            return BadRequest(new { Message = "CALENDAR_RANGE_INVALID" });

        var courseQuery = _context.Courses.AsNoTracking();
        if (!User.IsInRole(ApplicationRoles.Admin))
        {
            var trainerId = User.FindFirstValue("trainerId");
            if (string.IsNullOrWhiteSpace(trainerId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_TRAINER" });

            courseQuery = courseQuery.Where(c => c.TrainerId == trainerId);
        }

        var courseIds = await courseQuery.Select(c => c.CourseId).ToListAsync();
        if (courseIds.Count == 0)
            return Ok(new List<CourseCalendarItemDto>());

        return Ok(await BuildCalendarItems(courseIds, canManage: true, range.Value.From, range.Value.To));
    }

    private async Task<List<CourseCalendarItemDto>> BuildCalendarItems(
        List<string> courseIds,
        bool canManage,
        DateTime fromDate,
        DateTime toDate)
    {
        var items = new List<CourseCalendarItemDto>();
        var courseTitles = await _context.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.CourseId))
            .ToDictionaryAsync(c => c.CourseId, c => c.Title);

        var assignmentsQuery = _context.CourseAssignments
            .AsNoTracking()
            .Where(a => courseIds.Contains(a.CourseId) && a.DueAt != null && a.DueAt >= fromDate && a.DueAt <= toDate);
        if (!canManage)
            assignmentsQuery = assignmentsQuery.Where(a => a.Status == AssignmentStatus.Published);

        items.AddRange((await assignmentsQuery.ToListAsync()).Select(a => new CourseCalendarItemDto
        {
            ItemId = a.AssignmentId,
            CourseId = a.CourseId,
            CourseTitle = CourseTitle(courseTitles, a.CourseId),
            Type = "assignment",
            Title = a.Title,
            Description = a.Instructions,
            StartsAt = a.DueAt!.Value,
            Status = a.Status,
            Link = canManage ? $"/agency/assignments/{a.CourseId}" : $"/client/course-assignments/{a.CourseId}",
        }));

        var quizzesQuery = _context.CourseQuizzes
            .AsNoTracking()
            .Where(q => courseIds.Contains(q.CourseId));
        if (!canManage)
            quizzesQuery = quizzesQuery.Where(q => q.IsPublished);

        var quizzes = await quizzesQuery
            .Where(q =>
                (q.AvailableFrom != null && q.AvailableFrom >= fromDate && q.AvailableFrom <= toDate)
                || (q.AvailableUntil != null && q.AvailableUntil >= fromDate && q.AvailableUntil <= toDate)
                || (q.AvailableFrom == null && q.AvailableUntil == null))
            .ToListAsync();

        items.AddRange(quizzes.Select(q =>
        {
            var startsAt = q.AvailableUntil ?? q.AvailableFrom ?? DateTime.UtcNow;
            return new CourseCalendarItemDto
            {
                ItemId = q.QuizId,
                CourseId = q.CourseId,
                CourseTitle = CourseTitle(courseTitles, q.CourseId),
                Type = "quiz",
                Title = q.Title,
                Description = q.Description,
                StartsAt = startsAt,
                EndsAt = q.AvailableUntil,
                Status = q.IsPublished ? "Published" : "Draft",
                Link = canManage ? $"/agency/course-materials/{q.CourseId}" : $"/client/course-learn/{q.CourseId}",
            };
        }).Where(item => item.StartsAt >= fromDate && item.StartsAt <= toDate));

        var announcementsQuery = _context.CourseAnnouncements
            .AsNoTracking()
            .Where(a => courseIds.Contains(a.CourseId) && (a.PublishedAt ?? a.CreatedAt) >= fromDate && (a.PublishedAt ?? a.CreatedAt) <= toDate);
        if (!canManage)
            announcementsQuery = announcementsQuery.Where(a => a.IsPublished);

        items.AddRange((await announcementsQuery.ToListAsync()).Select(a => new CourseCalendarItemDto
        {
            ItemId = a.AnnouncementId,
            CourseId = a.CourseId,
            CourseTitle = CourseTitle(courseTitles, a.CourseId),
            Type = "announcement",
            Title = a.Title,
            Description = a.Body,
            StartsAt = a.PublishedAt ?? a.CreatedAt,
            Status = a.IsPublished ? "Published" : "Draft",
            Link = canManage ? $"/agency/announcements/{a.CourseId}" : $"/client/course-announcements/{a.CourseId}",
        }));

        var sessionsQuery = _context.CourseLiveSessions
            .AsNoTracking()
            .Where(s => courseIds.Contains(s.CourseId) && s.StartsAt >= fromDate && s.StartsAt <= toDate);
        if (!canManage)
            sessionsQuery = sessionsQuery.Where(s => s.Status != CourseLiveSessionStatus.Draft);

        items.AddRange((await sessionsQuery.ToListAsync()).Select(s => new CourseCalendarItemDto
        {
            ItemId = s.LiveSessionId,
            CourseId = s.CourseId,
            CourseTitle = CourseTitle(courseTitles, s.CourseId),
            Type = "live_session",
            Title = s.Title,
            Description = s.Description,
            StartsAt = s.StartsAt,
            EndsAt = s.EndsAt,
            Status = s.Status,
            Link = canManage ? $"/agency/live-sessions/{s.CourseId}" : $"/client/course-live-sessions/{s.CourseId}",
        }));

        return items
            .OrderBy(i => i.StartsAt)
            .ThenBy(i => i.Type)
            .ThenBy(i => i.Title)
            .ToList();
    }

    private async Task<List<string>> GetPaidCourseIdsAsync(string studentId)
    {
        var paidStatuses = new[] { PaymentStatus.Paid, PaymentStatus.Completed };
        var paymentCourseIds = await _context.Payments
            .AsNoTracking()
            .Where(p => p.StudentId == studentId && p.CourseId != null && paidStatuses.Contains(p.PaymentStatus!))
            .Select(p => p.CourseId!)
            .ToListAsync();

        var registrationCourseIds = await _context.CourseRegistrations
            .AsNoTracking()
            .Where(r => r.StudentId == studentId && paidStatuses.Contains(r.PaymentStatus!))
            .Select(r => r.CourseId)
            .ToListAsync();

        return paymentCourseIds
            .Concat(registrationCourseIds)
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private static (DateTime From, DateTime To)? BuildRange(DateTime? from, DateTime? to)
    {
        var fromDate = (from ?? DateTime.UtcNow.Date.AddDays(-7)).ToUniversalTime();
        var toDate = (to ?? fromDate.AddDays(60)).ToUniversalTime();
        return toDate < fromDate ? null : (fromDate, toDate);
    }

    private static string? CourseTitle(IReadOnlyDictionary<string, string?> titles, string courseId) =>
        titles.TryGetValue(courseId, out var title) ? title : null;
}
