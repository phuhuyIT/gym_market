using GymMarket.API.Data;
using GymMarket.API.DTOs.Admin;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/Admin/notifications")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminNotificationsController : ControllerBase
    {
        private static readonly string[] TemplateVariables =
        [
            "userName",
            "userEmail",
            "category",
            "title",
            "content",
            "link",
            "actionUrl"
        ];

        private readonly GymMarketContext _context;

        public AdminNotificationsController(GymMarketContext context)
        {
            _context = context;
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates()
        {
            var stored = await _context.NotificationTemplates
                .AsNoTracking()
                .ToListAsync();

            var templates = NotificationTypes.All
                .Select(type =>
                {
                    var template = stored.FirstOrDefault(item => item.Type == type);
                    return ToTemplateDto(type, template);
                })
                .ToList();

            return Ok(templates);
        }

        [HttpPut("templates/{type}")]
        public async Task<IActionResult> UpdateTemplate(string type, UpdateNotificationTemplateDto model)
        {
            if (!NotificationTypes.IsSupported(type))
            {
                return BadRequest(new { success = false, errors = new[] { "UNSUPPORTED_NOTIFICATION_TYPE" } });
            }

            if (string.IsNullOrWhiteSpace(model.SubjectTemplate) || string.IsNullOrWhiteSpace(model.BodyTemplate))
            {
                return BadRequest(new { success = false, errors = new[] { "TEMPLATE_SUBJECT_AND_BODY_REQUIRED" } });
            }

            var normalizedType = NotificationTypes.Normalize(type);
            var template = await _context.NotificationTemplates.FirstOrDefaultAsync(item => item.Type == normalizedType);
            var now = DateTime.UtcNow;

            if (template == null)
            {
                template = new NotificationTemplate
                {
                    Type = normalizedType,
                    CreatedAt = now,
                };
                _context.NotificationTemplates.Add(template);
            }

            template.SubjectTemplate = model.SubjectTemplate.Trim();
            template.BodyTemplate = model.BodyTemplate.Trim();
            template.IsActive = model.IsActive;
            template.UpdatedAt = now;
            template.UpdatedById = CurrentUserId();

            await _context.SaveChangesAsync();

            return Ok(ToTemplateDto(normalizedType, template));
        }

        [HttpGet("deliveries")]
        public async Task<IActionResult> GetDeliveries(
            [FromQuery] string? type,
            [FromQuery] string? channel,
            [FromQuery] string? status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 25)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 100) pageSize = 100;

            if (!string.IsNullOrWhiteSpace(type) && !NotificationTypes.IsSupported(type))
            {
                return BadRequest(new { success = false, errors = new[] { "UNSUPPORTED_NOTIFICATION_TYPE" } });
            }

            if (!string.IsNullOrWhiteSpace(channel) && !NotificationDeliveryChannels.IsSupported(channel))
            {
                return BadRequest(new { success = false, errors = new[] { "UNSUPPORTED_DELIVERY_CHANNEL" } });
            }

            if (!string.IsNullOrWhiteSpace(status) && !NotificationDeliveryStatuses.IsSupported(status))
            {
                return BadRequest(new { success = false, errors = new[] { "UNSUPPORTED_DELIVERY_STATUS" } });
            }

            var query = _context.NotificationDeliveryLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                var normalizedType = NotificationTypes.Normalize(type);
                query = query.Where(log => log.Type == normalizedType);
            }

            if (!string.IsNullOrWhiteSpace(channel))
            {
                var normalizedChannel = NotificationDeliveryChannels.Normalize(channel);
                query = query.Where(log => log.Channel == normalizedChannel);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = NotificationDeliveryStatuses.Normalize(status);
                query = query.Where(log => log.Status == normalizedStatus);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(log => log.CreatedAt)
                .ThenByDescending(log => log.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(log => new NotificationDeliveryLogDto
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    RecipientEmail = log.RecipientEmail,
                    RecipientName = log.RecipientName,
                    Type = log.Type,
                    TypeLabel = NotificationTypes.LabelFor(log.Type),
                    Channel = log.Channel,
                    Status = log.Status,
                    Subject = log.Subject,
                    Content = log.Content,
                    Link = log.Link,
                    ErrorMessage = log.ErrorMessage,
                    CreatedAt = log.CreatedAt,
                })
                .ToListAsync();

            foreach (var item in items)
            {
                item.CreatedAt = DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc);
            }

            return Ok(new PagedResult<NotificationDeliveryLogDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
            });
        }

        private static AdminNotificationTemplateDto ToTemplateDto(string type, NotificationTemplate? template)
        {
            return new AdminNotificationTemplateDto
            {
                Type = type,
                Label = NotificationTypes.LabelFor(type),
                SubjectTemplate = template?.SubjectTemplate ?? "{{title}}",
                BodyTemplate = template?.BodyTemplate ?? DefaultBodyTemplate(),
                IsActive = template?.IsActive ?? true,
                UpdatedAt = template?.UpdatedAt,
                Variables = TemplateVariables,
            };
        }

        private static string DefaultBodyTemplate()
        {
            return """
                <p>Hello {{userName}},</p>
                <p>{{content}}</p>
                <p><a href="{{actionUrl}}">Open in GymMarket</a></p>
                <p>You can change delivery preferences in account settings.</p>
                """;
        }

        private string? CurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
