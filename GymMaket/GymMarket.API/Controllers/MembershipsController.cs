using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Membership;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MembershipsController : ControllerBase
{
    private readonly GymMarketContext _context;

    public MembershipsController(GymMarketContext context)
    {
        _context = context;
    }

    [HttpGet("plans")]
    public async Task<ActionResult<List<MembershipPlanDto>>> GetPlans([FromQuery] bool includeInactive = false)
    {
        var plans = await _context.MembershipPlans
            .Where(p => includeInactive || p.IsActive)
            .OrderBy(p => p.Price)
            .ThenBy(p => p.DurationDays)
            .Select(p => ToPlanDto(p))
            .ToListAsync();

        return Ok(plans);
    }

    [HttpPost("plans")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<MembershipPlanDto>> CreatePlan(UpsertMembershipPlanDto dto)
    {
        var validation = ValidatePlan(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var plan = new MembershipPlan
        {
            PlanId = Guid.NewGuid().ToString(),
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            DurationDays = dto.DurationDays,
            Price = dto.Price,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.MembershipPlans.Add(plan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPlans), new { id = plan.PlanId }, ToPlanDto(plan));
    }

    [HttpPut("plans/{planId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<MembershipPlanDto>> UpdatePlan(string planId, UpsertMembershipPlanDto dto)
    {
        var validation = ValidatePlan(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var plan = await _context.MembershipPlans.FirstOrDefaultAsync(p => p.PlanId == planId);
        if (plan == null)
        {
            return NotFound(new { Message = "MEMBERSHIP_PLAN_NOT_FOUND" });
        }

        plan.Name = dto.Name.Trim();
        plan.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        plan.DurationDays = dto.DurationDays;
        plan.Price = dto.Price;
        plan.IsActive = dto.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ToPlanDto(plan));
    }

    [HttpDelete("plans/{planId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> DeactivatePlan(string planId)
    {
        var plan = await _context.MembershipPlans.FirstOrDefaultAsync(p => p.PlanId == planId);
        if (plan == null)
        {
            return NotFound(new { Message = "MEMBERSHIP_PLAN_NOT_FOUND" });
        }

        plan.IsActive = false;
        plan.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "MEMBERSHIP_PLAN_DEACTIVATED" });
    }

    [HttpGet("me/status")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<MembershipStatusDto>> GetMyStatus()
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        await ExpireOldMemberships(studentId);

        var now = DateTime.UtcNow;
        var current = await _context.StudentMemberships
            .Include(m => m.Plan)
            .Include(m => m.Student)
            .Where(m => m.StudentId == studentId
                && m.Status == MembershipStatus.Active
                && m.StartsAt <= now
                && m.EndsAt >= now)
            .OrderByDescending(m => m.EndsAt)
            .FirstOrDefaultAsync();

        var plans = await _context.MembershipPlans
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .ThenBy(p => p.DurationDays)
            .Select(p => ToPlanDto(p))
            .ToListAsync();

        return Ok(new MembershipStatusDto
        {
            HasActiveMembership = current != null,
            CurrentMembership = current == null ? null : ToMembershipDto(current),
            AvailablePlans = plans
        });
    }

    [HttpPost("subscribe")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentMembershipDto>> Subscribe(SubscribeMembershipDto dto)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var membership = await CreateMembership(studentId, dto.PlanId);
        if (membership == null)
        {
            return NotFound(new { Message = "MEMBERSHIP_PLAN_NOT_FOUND" });
        }

        return Ok(ToMembershipDto(membership));
    }

    [HttpPost("me/cancel")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CancelMine()
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var now = DateTime.UtcNow;
        var current = await _context.StudentMemberships
            .Where(m => m.StudentId == studentId
                && m.Status == MembershipStatus.Active
                && m.StartsAt <= now
                && m.EndsAt >= now)
            .OrderByDescending(m => m.EndsAt)
            .FirstOrDefaultAsync();

        if (current == null)
        {
            return NotFound(new { Message = "ACTIVE_MEMBERSHIP_NOT_FOUND" });
        }

        current.Status = MembershipStatus.Cancelled;
        current.CancelledAt = now;
        current.UpdatedAt = now;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "MEMBERSHIP_CANCELLED" });
    }

    [HttpGet("subscriptions")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<List<StudentMembershipDto>>> GetSubscriptions([FromQuery] string status = "")
    {
        await ExpireOldMemberships();

        var query = _context.StudentMemberships
            .Include(m => m.Plan)
            .Include(m => m.Student)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(m => m.Status == status.Trim());
        }

        var subscriptions = await query
            .OrderByDescending(m => m.EndsAt)
            .Take(200)
            .ToListAsync();

        return Ok(subscriptions.Select(ToMembershipDto).ToList());
    }

    [HttpPost("students/{studentId}/assign")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<StudentMembershipDto>> AssignToStudent(string studentId, AssignMembershipDto dto)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);
        if (!studentExists)
        {
            return NotFound(new { Message = "STUDENT_NOT_FOUND" });
        }

        var membership = await CreateMembership(studentId, dto.PlanId, dto.StartsAt);
        if (membership == null)
        {
            return NotFound(new { Message = "MEMBERSHIP_PLAN_NOT_FOUND" });
        }

        return Ok(ToMembershipDto(membership));
    }

    private async Task<StudentMembership?> CreateMembership(string studentId, string planId, DateTime? requestedStart = null)
    {
        if (string.IsNullOrWhiteSpace(planId))
        {
            return null;
        }

        var plan = await _context.MembershipPlans.FirstOrDefaultAsync(p => p.PlanId == planId && p.IsActive);
        if (plan == null)
        {
            return null;
        }

        await ExpireOldMemberships(studentId);

        var now = DateTime.UtcNow;
        var activeMembership = await _context.StudentMemberships
            .Where(m => m.StudentId == studentId && m.Status == MembershipStatus.Active && m.EndsAt >= now)
            .OrderByDescending(m => m.EndsAt)
            .FirstOrDefaultAsync();

        var requested = requestedStart?.ToUniversalTime() ?? now;
        var startsAt = activeMembership?.EndsAt ?? requested;
        if (startsAt < now)
        {
            startsAt = now;
        }

        var membership = new StudentMembership
        {
            MembershipId = Guid.NewGuid().ToString(),
            PlanId = plan.PlanId,
            StudentId = studentId,
            Status = MembershipStatus.Active,
            StartsAt = startsAt,
            EndsAt = startsAt.AddDays(plan.DurationDays),
            CreatedAt = now,
            Plan = plan
        };

        _context.StudentMemberships.Add(membership);
        await _context.SaveChangesAsync();

        membership.Student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        return membership;
    }

    private async Task ExpireOldMemberships(string? studentId = null)
    {
        var now = DateTime.UtcNow;
        var query = _context.StudentMemberships
            .Where(m => m.Status == MembershipStatus.Active && m.EndsAt < now);

        if (!string.IsNullOrWhiteSpace(studentId))
        {
            query = query.Where(m => m.StudentId == studentId);
        }

        var memberships = await query.ToListAsync();
        if (memberships.Count == 0)
        {
            return;
        }

        foreach (var membership in memberships)
        {
            membership.Status = MembershipStatus.Expired;
            membership.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();
    }

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }

    private static string? ValidatePlan(UpsertMembershipPlanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return "MEMBERSHIP_PLAN_NAME_REQUIRED";
        }

        if (dto.DurationDays <= 0)
        {
            return "MEMBERSHIP_PLAN_DURATION_INVALID";
        }

        if (dto.Price < 0)
        {
            return "MEMBERSHIP_PLAN_PRICE_INVALID";
        }

        return null;
    }

    private static MembershipPlanDto ToPlanDto(MembershipPlan plan)
    {
        return new MembershipPlanDto
        {
            PlanId = plan.PlanId,
            Name = plan.Name,
            Description = plan.Description,
            DurationDays = plan.DurationDays,
            Price = plan.Price,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }

    private static StudentMembershipDto ToMembershipDto(StudentMembership membership)
    {
        return new StudentMembershipDto
        {
            MembershipId = membership.MembershipId,
            PlanId = membership.PlanId,
            PlanName = membership.Plan?.Name ?? string.Empty,
            StudentId = membership.StudentId,
            StudentName = membership.Student?.Name,
            StudentEmail = membership.Student?.Email,
            Status = membership.Status,
            StartsAt = membership.StartsAt,
            EndsAt = membership.EndsAt,
            CancelledAt = membership.CancelledAt,
            Price = membership.Plan?.Price ?? 0,
            DurationDays = membership.Plan?.DurationDays ?? 0
        };
    }
}
