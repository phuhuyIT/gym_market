using GymMarket.API.Data;
using GymMarket.API.DTOs.Admin;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/Admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private const string ActiveStatus = "Active";
        private const string SuspendedStatus = "Suspended";
        private static readonly string[] SupportedStatuses = [ActiveStatus, SuspendedStatus];
        private static readonly string[] SupportedRoles = [ApplicationRoles.Admin, ApplicationRoles.Trainer, ApplicationRoles.Student];

        private readonly GymMarketContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccountService _accountService;

        public AdminUsersController(
            GymMarketContext context,
            UserManager<AppUser> userManager,
            IAccountService accountService)
        {
            _context = context;
            _userManager = userManager;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] string? role,
            [FromQuery] string? status,
            [FromQuery] string? trainerApprovalStatus,
            [FromQuery] bool? emailConfirmed,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = Defaults.PageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = Defaults.PageSize;
            if (pageSize > 100) pageSize = 100;

            var normalizedRole = NormalizeRole(role);
            if (!string.IsNullOrWhiteSpace(role) && normalizedRole == null)
                return BadRequest(new { success = false, errors = new[] { "INVALID_ROLE_FILTER" } });

            var normalizedStatus = NormalizeStatus(status);
            if (!string.IsNullOrWhiteSpace(status) && normalizedStatus == null)
                return BadRequest(new { success = false, errors = new[] { "INVALID_STATUS_FILTER" } });

            var normalizedTrainerApprovalStatus = TrainerApprovalStatus.TryNormalize(trainerApprovalStatus);
            if (!string.IsNullOrWhiteSpace(trainerApprovalStatus) && normalizedTrainerApprovalStatus == null)
                return BadRequest(new { success = false, errors = new[] { "INVALID_TRAINER_APPROVAL_STATUS_FILTER" } });

            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.Contains(term)) ||
                    (u.Email != null && u.Email.Contains(term)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(term)));
            }

            if (normalizedStatus != null)
            {
                query = normalizedStatus == ActiveStatus
                    ? query.Where(u => u.Status == null || u.Status == "" || u.Status == ActiveStatus)
                    : query.Where(u => u.Status == normalizedStatus);
            }

            if (emailConfirmed.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == emailConfirmed.Value);
            }

            if (normalizedTrainerApprovalStatus != null)
            {
                query = query.Where(u => _context.Trainers.Any(t =>
                    t.UserId == u.Id &&
                    (t.ApprovalStatus == normalizedTrainerApprovalStatus ||
                     (normalizedTrainerApprovalStatus == TrainerApprovalStatus.PendingReview &&
                      (t.ApprovalStatus == null || t.ApprovalStatus == "")))));
            }

            if (normalizedRole != null)
            {
                var roleId = await _context.Roles
                    .Where(r => r.Name == normalizedRole)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (roleId == null)
                {
                    return Ok(new PagedResult<AdminUserListItemDto>
                    {
                        Items = [],
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalCount = 0
                    });
                }

                query = query.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FullName ?? u.Email)
                .ThenBy(u => u.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = await BuildListItems(users);
            return Ok(new PagedResult<AdminUserListItemDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, errors = new[] { "USER_NOT_FOUND" } });

            return Ok(await BuildDetail(user));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, UpdateUserStatusDto model)
        {
            var normalizedStatus = NormalizeStatus(model.Status);
            if (normalizedStatus == null)
                return BadRequest(new { success = false, errors = new[] { "INVALID_STATUS" } });

            if (id == CurrentUserId() && normalizedStatus == SuspendedStatus)
                return BadRequest(new { success = false, errors = new[] { "CANNOT_SUSPEND_SELF" } });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, errors = new[] { "USER_NOT_FOUND" } });

            user.Status = normalizedStatus;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = result.Errors.Select(e => e.Description).ToArray()
                });
            }

            var stampResult = await _userManager.UpdateSecurityStampAsync(user);
            if (!stampResult.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = stampResult.Errors.Select(e => e.Description).ToArray()
                });
            }

            if (normalizedStatus == SuspendedStatus)
            {
                await RevokeRefreshTokens(id);
            }

            return Ok(new { success = true, message = "USER_STATUS_UPDATED" });
        }

        [HttpPut("{id}/trainer-approval")]
        public async Task<IActionResult> UpdateTrainerApproval(string id, UpdateTrainerApprovalDto model)
        {
            var normalizedStatus = TrainerApprovalStatus.TryNormalize(model.Status);
            if (normalizedStatus == null)
                return BadRequest(new { success = false, errors = new[] { "INVALID_TRAINER_APPROVAL_STATUS" } });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, errors = new[] { "USER_NOT_FOUND" } });

            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == id);
            if (trainer == null)
                return NotFound(new { success = false, errors = new[] { "TRAINER_PROFILE_NOT_FOUND" } });

            trainer.ApprovalStatus = normalizedStatus;
            trainer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "TRAINER_APPROVAL_UPDATED" });
        }

        [HttpPost("{id}/resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation(string id)
        {
            var response = await _accountService.SendEmailConfirmation(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Success });
        }

        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, errors = new[] { "USER_NOT_FOUND" } });

            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);
            return Ok(new { success = true, message = "ACCOUNT_UNLOCKED" });
        }

        private async Task<IReadOnlyList<AdminUserListItemDto>> BuildListItems(IReadOnlyList<AppUser> users)
        {
            var userIds = users.Select(u => u.Id).ToArray();
            var studentIds = await _context.Students
                .AsNoTracking()
                .Where(s => s.UserId != null && userIds.Contains(s.UserId))
                .ToDictionaryAsync(s => s.UserId!, s => s.StudentId);

            var trainerProfiles = await _context.Trainers
                .AsNoTracking()
                .Where(t => t.UserId != null && userIds.Contains(t.UserId))
                .Select(t => new
                {
                    t.UserId,
                    t.TrainerId,
                    t.ApprovalStatus
                })
                .ToDictionaryAsync(
                    t => t.UserId!,
                    t => new TrainerProfileSummary(
                        t.TrainerId,
                        TrainerApprovalStatus.NormalizeStored(t.ApprovalStatus)));

            var items = new List<AdminUserListItemDto>();
            foreach (var user in users)
            {
                items.Add(await BuildListItem(user, studentIds, trainerProfiles));
            }

            return items;
        }

        private async Task<AdminUserListItemDto> BuildListItem(
            AppUser user,
            IReadOnlyDictionary<string, string> studentIds,
            IReadOnlyDictionary<string, TrainerProfileSummary> trainerProfiles)
        {
            var roles = await _userManager.GetRolesAsync(user);
            trainerProfiles.TryGetValue(user.Id, out var trainerProfile);
            return new AdminUserListItemDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = EffectiveStatus(user.Status),
                EmailConfirmed = user.EmailConfirmed,
                IsLockedOut = await _userManager.IsLockedOutAsync(user),
                LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
                Roles = roles.ToArray(),
                StudentId = studentIds.TryGetValue(user.Id, out var studentId) ? studentId : null,
                TrainerId = trainerProfile?.TrainerId,
                TrainerApprovalStatus = trainerProfile?.ApprovalStatus
            };
        }

        private async Task<AdminUserDetailDto> BuildDetail(AppUser user)
        {
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.Id);
            var trainer = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.UserId == user.Id);
            var roles = await _userManager.GetRolesAsync(user);

            return new AdminUserDetailDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Avatar = user.Avatar,
                Status = EffectiveStatus(user.Status),
                EmailConfirmed = user.EmailConfirmed,
                IsLockedOut = await _userManager.IsLockedOutAsync(user),
                LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
                AccessFailedCount = await _userManager.GetAccessFailedCountAsync(user),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                Roles = roles.ToArray(),
                StudentId = student?.StudentId,
                TrainerId = trainer?.TrainerId,
                TrainerApprovalStatus = trainer == null
                    ? null
                    : TrainerApprovalStatus.NormalizeStored(trainer.ApprovalStatus)
            };
        }

        private async Task RevokeRefreshTokens(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }

        private static string EffectiveStatus(string? status)
        {
            return string.IsNullOrWhiteSpace(status) ? ActiveStatus : status;
        }

        private static string? NormalizeStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            return SupportedStatuses.FirstOrDefault(
                supported => string.Equals(supported, status.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private static string? NormalizeRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return null;

            return SupportedRoles.FirstOrDefault(
                supported => string.Equals(supported, role.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private string? CurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private sealed record TrainerProfileSummary(string TrainerId, string ApprovalStatus);
    }
}
