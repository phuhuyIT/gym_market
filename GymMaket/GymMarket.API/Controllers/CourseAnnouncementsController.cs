using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Announcements;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseAnnouncementsController : ControllerBase
{
    private readonly GymMarketContext _context;
    private readonly ICourseAccessService _courseAccessService;
    private readonly INotificationRepository _notificationRepository;
    private readonly UserManager<AppUser> _userManager;

    public CourseAnnouncementsController(
        GymMarketContext context,
        ICourseAccessService courseAccessService,
        INotificationRepository notificationRepository,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _courseAccessService = courseAccessService;
        _notificationRepository = notificationRepository;
        _userManager = userManager;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseAnnouncements(string courseId)
    {
        var canManage = await _courseAccessService.CanManageCourseAsync(User, courseId);
        if (!canManage && !await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var query = _context.CourseAnnouncements
            .AsNoTracking()
            .Where(a => a.CourseId == courseId);

        if (!canManage)
            query = query.Where(a => a.IsPublished);

        var announcements = await query
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.PublishedAt ?? a.CreatedAt)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(announcements.Select(ToDto));
    }

    [HttpGet("{announcementId}")]
    public async Task<IActionResult> GetAnnouncement(string announcementId)
    {
        var announcement = await _context.CourseAnnouncements
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);
        if (announcement == null)
            return NotFound(new { Message = "ANNOUNCEMENT_NOT_FOUND" });

        var canManage = await _courseAccessService.CanManageCourseAsync(User, announcement.CourseId);
        if (!canManage)
        {
            if (!announcement.IsPublished || !await _courseAccessService.CanAccessCourseAsync(User, announcement.CourseId))
                return Forbid();
        }

        return Ok(ToDto(announcement));
    }

    [HttpPost("course/{courseId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> CreateAnnouncement(string courseId, UpsertCourseAnnouncementDto dto)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var validationError = Validate(dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var author = await GetAuthorAsync();
        var now = DateTime.UtcNow;
        var announcement = new CourseAnnouncement
        {
            AnnouncementId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            CreatedByUserId = CurrentUserId(),
            CreatedByRole = author.Role,
            CreatedByName = author.Name,
            Title = dto.Title.Trim(),
            Body = dto.Body.Trim(),
            IsPinned = dto.IsPinned,
            IsPublished = dto.IsPublished,
            PublishedAt = dto.IsPublished ? now : null,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _context.CourseAnnouncements.Add(announcement);
        await _context.SaveChangesAsync();

        if (announcement.IsPublished)
            await NotifyLearnersAsync(announcement);

        return Ok(ToDto(announcement));
    }

    [HttpPut("{announcementId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> UpdateAnnouncement(string announcementId, UpsertCourseAnnouncementDto dto)
    {
        var announcement = await _context.CourseAnnouncements
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);
        if (announcement == null)
            return NotFound(new { Message = "ANNOUNCEMENT_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, announcement.CourseId))
            return Forbid();

        var validationError = Validate(dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var shouldNotify = !announcement.IsPublished && dto.IsPublished;
        announcement.Title = dto.Title.Trim();
        announcement.Body = dto.Body.Trim();
        announcement.IsPinned = dto.IsPinned;
        announcement.IsPublished = dto.IsPublished;
        announcement.PublishedAt = dto.IsPublished
            ? announcement.PublishedAt ?? DateTime.UtcNow
            : null;
        announcement.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        if (shouldNotify)
            await NotifyLearnersAsync(announcement);

        return Ok(ToDto(announcement));
    }

    [HttpDelete("{announcementId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> DeleteAnnouncement(string announcementId)
    {
        var announcement = await _context.CourseAnnouncements
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);
        if (announcement == null)
            return NotFound(new { Message = "ANNOUNCEMENT_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, announcement.CourseId))
            return Forbid();

        _context.CourseAnnouncements.Remove(announcement);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task NotifyLearnersAsync(CourseAnnouncement announcement)
    {
        var paidStatuses = new[] { PaymentStatus.Paid, PaymentStatus.Completed };
        var paymentUserIds = await _context.Payments
            .AsNoTracking()
            .Where(p => p.CourseId == announcement.CourseId && paidStatuses.Contains(p.PaymentStatus!))
            .Select(p => p.Student!.UserId)
            .ToListAsync();

        var registrationUserIds = await _context.CourseRegistrations
            .AsNoTracking()
            .Where(r => r.CourseId == announcement.CourseId && paidStatuses.Contains(r.PaymentStatus!))
            .Select(r => r.Student!.UserId)
            .ToListAsync();

        var targetUserIds = paymentUserIds
            .Concat(registrationUserIds)
            .Where(userId => !string.IsNullOrWhiteSpace(userId))
            .Distinct()
            .Cast<string>()
            .ToList();

        if (targetUserIds.Count == 0)
            return;

        await _notificationRepository.NotifyUsers(
            targetUserIds,
            NotificationTypes.Announcement,
            announcement.Title,
            TrimForNotification(announcement.Body),
            $"/client/course-announcements/{announcement.CourseId}");
    }

    private async Task<(string Role, string Name)> GetAuthorAsync()
    {
        var userId = CurrentUserId();
        if (User.IsInRole(DiscussionAuthorRole.Admin))
        {
            var user = string.IsNullOrWhiteSpace(userId) ? null : await _userManager.FindByIdAsync(userId);
            return (DiscussionAuthorRole.Admin, user?.FullName ?? user?.Email ?? "Admin");
        }

        var trainerId = User.FindFirstValue("trainerId");
        if (!string.IsNullOrWhiteSpace(trainerId))
        {
            var trainer = await _context.Trainers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TrainerId == trainerId);
            if (trainer != null)
                return (DiscussionAuthorRole.Trainer, trainer.Name ?? trainer.Email ?? "Trainer");
        }

        return (DiscussionAuthorRole.Trainer, "Trainer");
    }

    private static string? Validate(UpsertCourseAnnouncementDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return "ANNOUNCEMENT_TITLE_REQUIRED";

        if (string.IsNullOrWhiteSpace(dto.Body))
            return "ANNOUNCEMENT_BODY_REQUIRED";

        return null;
    }

    private static CourseAnnouncementDto ToDto(CourseAnnouncement announcement) => new()
    {
        AnnouncementId = announcement.AnnouncementId,
        CourseId = announcement.CourseId,
        CreatedByUserId = announcement.CreatedByUserId,
        CreatedByRole = announcement.CreatedByRole,
        CreatedByName = announcement.CreatedByName,
        Title = announcement.Title,
        Body = announcement.Body,
        IsPinned = announcement.IsPinned,
        IsPublished = announcement.IsPublished,
        PublishedAt = announcement.PublishedAt,
        CreatedAt = announcement.CreatedAt,
        UpdatedAt = announcement.UpdatedAt,
    };

    private static string TrimForNotification(string body)
    {
        var trimmed = body.Trim();
        return trimmed.Length <= 240 ? trimmed : trimmed[..237] + "...";
    }

    private string? CurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
