using GymMarket.API.DTOs.Notifications;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet("get-notifications")]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int take = 50,
            [FromQuery] int skip = 0,
            [FromQuery] string? type = null,
            [FromQuery] bool? isRead = null)
        {
            var notifications = await _notificationRepository.GetNotificationsOfUser(GetUserId(), take, skip, type, isRead);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _notificationRepository.GetUnreadCount(GetUserId());
            return Ok(count);
        }

        [HttpPost("mark-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationRepository.MarkAsRead(id, GetUserId());
            return Ok();
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationRepository.MarkAllAsRead(GetUserId());
            return Ok();
        }

        [HttpPost("mark-type-read/{type}")]
        public async Task<IActionResult> MarkTypeAsRead(string type)
        {
            await _notificationRepository.MarkTypeAsRead(GetUserId(), type);
            return Ok();
        }

        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            var preferences = await _notificationRepository.GetPreferences(GetUserId());
            return Ok(preferences);
        }

        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences(UpdateNotificationPreferencesDto model)
        {
            if (model.Preferences.Count == 0)
            {
                return BadRequest(new { message = "At least one preference is required." });
            }

            var invalidTypes = model.Preferences
                .Select(p => p.Type)
                .Where(type => !NotificationTypes.IsSupported(type))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (invalidTypes.Count > 0)
            {
                return BadRequest(new { message = "Unsupported notification type.", types = invalidTypes });
            }

            var invalidFrequencies = model.Preferences
                .Where(p => !string.IsNullOrWhiteSpace(p.EmailFrequency)
                    && !NotificationEmailFrequencies.IsSupported(p.EmailFrequency))
                .Select(p => p.EmailFrequency)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (invalidFrequencies.Count > 0)
            {
                return BadRequest(new { message = "Unsupported email notification frequency.", frequencies = invalidFrequencies });
            }

            var preferences = await _notificationRepository.UpdatePreferences(GetUserId(), model.Preferences);
            return Ok(preferences);
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }
    }
}
