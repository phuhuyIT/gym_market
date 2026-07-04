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

        var fromDate = (from ?? DateTime.UtcNow.Date.AddDays(-7)).ToUniversalTime();
        var toDate = (to ?? fromDate.AddDays(60)).ToUniversalTime();
        if (toDate < fromDate)
            return BadRequest(new { Message = "CALENDAR_RANGE_INVALID" });

        var items = new List<CourseCalendarItemDto>();

        var assignmentsQuery = _context.CourseAssignments
            .AsNoTracking()
            .Where(a => a.CourseId == courseId && a.DueAt != null && a.DueAt >= fromDate && a.DueAt <= toDate);
        if (!canManage)
            assignmentsQuery = assignmentsQuery.Where(a => a.Status == AssignmentStatus.Published);

        items.AddRange((await assignmentsQuery.ToListAsync()).Select(a => new CourseCalendarItemDto
        {
            ItemId = a.AssignmentId,
            CourseId = a.CourseId,
            Type = "assignment",
            Title = a.Title,
            Description = a.Instructions,
            StartsAt = a.DueAt!.Value,
            Status = a.Status,
            Link = canManage ? $"/agency/assignments/{courseId}" : $"/client/course-assignments/{courseId}",
        }));

        var quizzesQuery = _context.CourseQuizzes
            .AsNoTracking()
            .Where(q => q.CourseId == courseId);
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
                Type = "quiz",
                Title = q.Title,
                Description = q.Description,
                StartsAt = startsAt,
                EndsAt = q.AvailableUntil,
                Status = q.IsPublished ? "Published" : "Draft",
                Link = canManage ? $"/agency/course-materials/{courseId}" : $"/client/course-learn/{courseId}",
            };
        }).Where(item => item.StartsAt >= fromDate && item.StartsAt <= toDate));

        var announcementsQuery = _context.CourseAnnouncements
            .AsNoTracking()
            .Where(a => a.CourseId == courseId && (a.PublishedAt ?? a.CreatedAt) >= fromDate && (a.PublishedAt ?? a.CreatedAt) <= toDate);
        if (!canManage)
            announcementsQuery = announcementsQuery.Where(a => a.IsPublished);

        items.AddRange((await announcementsQuery.ToListAsync()).Select(a => new CourseCalendarItemDto
        {
            ItemId = a.AnnouncementId,
            CourseId = a.CourseId,
            Type = "announcement",
            Title = a.Title,
            Description = a.Body,
            StartsAt = a.PublishedAt ?? a.CreatedAt,
            Status = a.IsPublished ? "Published" : "Draft",
            Link = canManage ? $"/agency/announcements/{courseId}" : $"/client/course-announcements/{courseId}",
        }));

        var sessionsQuery = _context.CourseLiveSessions
            .AsNoTracking()
            .Where(s => s.CourseId == courseId && s.StartsAt >= fromDate && s.StartsAt <= toDate);
        if (!canManage)
            sessionsQuery = sessionsQuery.Where(s => s.Status != CourseLiveSessionStatus.Draft);

        items.AddRange((await sessionsQuery.ToListAsync()).Select(s => new CourseCalendarItemDto
        {
            ItemId = s.LiveSessionId,
            CourseId = s.CourseId,
            Type = "live_session",
            Title = s.Title,
            Description = s.Description,
            StartsAt = s.StartsAt,
            EndsAt = s.EndsAt,
            Status = s.Status,
            Link = canManage ? $"/agency/live-sessions/{courseId}" : $"/client/course-live-sessions/{courseId}",
        }));

        return Ok(items
            .OrderBy(i => i.StartsAt)
            .ThenBy(i => i.Type)
            .ThenBy(i => i.Title)
            .ToList());
    }
}
