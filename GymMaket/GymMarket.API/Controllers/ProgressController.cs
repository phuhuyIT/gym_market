using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Progress;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly GymMarketContext _context;
    private readonly INotificationRepository _notificationRepository;

    public ProgressController(GymMarketContext context, INotificationRepository notificationRepository)
    {
        _context = context;
        _notificationRepository = notificationRepository;
    }

    [HttpGet("me/logs")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<ProgressLogDto>>> GetMyLogs()
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var logs = await _context.StudentProgressLogs
            .Include(l => l.Student)
            .Where(l => l.StudentId == studentId)
            .OrderByDescending(l => l.LoggedAt)
            .Take(100)
            .ToListAsync();

        return Ok(logs.Select(ToLogDto).ToList());
    }

    [HttpPost("me/logs")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ProgressLogDto>> CreateMyLog(UpsertProgressLogDto dto)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var validation = ValidateLog(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var log = new StudentProgressLog
        {
            ProgressLogId = Guid.NewGuid().ToString(),
            StudentId = studentId,
            LoggedAt = (dto.LoggedAt ?? DateTime.UtcNow).ToUniversalTime(),
            WeightKg = dto.WeightKg,
            BodyFatPercent = dto.BodyFatPercent,
            WaistCm = dto.WaistCm,
            ChestCm = dto.ChestCm,
            ArmCm = dto.ArmCm,
            HipCm = dto.HipCm,
            StrengthNotes = string.IsNullOrWhiteSpace(dto.StrengthNotes) ? null : dto.StrengthNotes.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            CreatedAt = DateTime.UtcNow,
            Student = student
        };

        _context.StudentProgressLogs.Add(log);
        await _context.SaveChangesAsync();

        await NotifyAssignedTrainers(
            studentId,
            "Progress log added",
            $"{student.Name ?? "A student"} added a new progress check-in.",
            "/agency/progress");

        return Ok(ToLogDto(log));
    }

    [HttpGet("me/goal")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ProgressGoalDto?>> GetMyGoal()
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var goal = await _context.StudentProgressGoals
            .Where(g => g.StudentId == studentId && g.Status == ProgressGoalStatus.Active)
            .OrderByDescending(g => g.CreatedAt)
            .FirstOrDefaultAsync();

        return Ok(goal == null ? null : ToGoalDto(goal));
    }

    [HttpPut("me/goal")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ProgressGoalDto>> UpsertMyGoal(UpsertProgressGoalDto dto)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var validation = ValidateGoal(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var goal = await _context.StudentProgressGoals
            .Where(g => g.StudentId == studentId && g.Status == ProgressGoalStatus.Active)
            .OrderByDescending(g => g.CreatedAt)
            .FirstOrDefaultAsync();
        var now = DateTime.UtcNow;

        if (goal == null)
        {
            goal = new StudentProgressGoal
            {
                ProgressGoalId = Guid.NewGuid().ToString(),
                StudentId = studentId,
                CreatedAt = now
            };
            _context.StudentProgressGoals.Add(goal);
        }

        goal.TargetWeightKg = dto.TargetWeightKg;
        goal.TargetBodyFatPercent = dto.TargetBodyFatPercent;
        goal.GoalDate = dto.GoalDate?.ToUniversalTime();
        goal.Status = NormalizeGoalStatus(dto.Status);
        goal.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
        goal.UpdatedAt = now;

        await _context.SaveChangesAsync();

        var studentName = await _context.Students
            .Where(s => s.StudentId == studentId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync();

        await NotifyAssignedTrainers(
            studentId,
            "Progress goal updated",
            $"{studentName ?? "A student"} updated their progress goal.",
            "/agency/progress");

        return Ok(ToGoalDto(goal));
    }

    [HttpGet("summaries")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<List<ProgressSummaryDto>>> GetSummaries()
    {
        var studentIds = await ReviewableStudentIds();
        if (studentIds == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_TRAINER" });
        }

        var summaries = await BuildSummaries(studentIds);
        return Ok(summaries);
    }

    [HttpGet("students/{studentId}/logs")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<List<ProgressLogDto>>> GetStudentLogs(string studentId)
    {
        if (!await CanReviewStudent(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "PROGRESS_REVIEW_FORBIDDEN" });
        }

        var logs = await _context.StudentProgressLogs
            .Include(l => l.Student)
            .Where(l => l.StudentId == studentId)
            .OrderByDescending(l => l.LoggedAt)
            .Take(100)
            .ToListAsync();

        return Ok(logs.Select(ToLogDto).ToList());
    }

    [HttpGet("students/{studentId}/goal")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<ProgressGoalDto?>> GetStudentGoal(string studentId)
    {
        if (!await CanReviewStudent(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "PROGRESS_REVIEW_FORBIDDEN" });
        }

        var goal = await _context.StudentProgressGoals
            .Where(g => g.StudentId == studentId && g.Status == ProgressGoalStatus.Active)
            .OrderByDescending(g => g.CreatedAt)
            .FirstOrDefaultAsync();

        return Ok(goal == null ? null : ToGoalDto(goal));
    }

    private async Task<List<ProgressSummaryDto>> BuildSummaries(List<string> studentIds)
    {
        var students = await _context.Students
            .Where(s => studentIds.Contains(s.StudentId))
            .OrderBy(s => s.Name)
            .ToListAsync();
        var logs = await _context.StudentProgressLogs
            .Where(l => studentIds.Contains(l.StudentId))
            .OrderBy(l => l.LoggedAt)
            .ToListAsync();
        var goals = await _context.StudentProgressGoals
            .Where(g => studentIds.Contains(g.StudentId) && g.Status == ProgressGoalStatus.Active)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();

        return students.Select(student =>
        {
            var studentLogs = logs.Where(l => l.StudentId == student.StudentId).OrderBy(l => l.LoggedAt).ToList();
            var first = studentLogs.FirstOrDefault();
            var latest = studentLogs.LastOrDefault();
            var goal = goals.FirstOrDefault(g => g.StudentId == student.StudentId);

            return new ProgressSummaryDto
            {
                StudentId = student.StudentId,
                StudentName = student.Name,
                StudentEmail = student.Email,
                LogCount = studentLogs.Count,
                LatestLoggedAt = latest?.LoggedAt,
                LatestWeightKg = latest?.WeightKg,
                WeightChangeKg = latest?.WeightKg - first?.WeightKg,
                LatestBodyFatPercent = latest?.BodyFatPercent,
                BodyFatChangePercent = latest?.BodyFatPercent - first?.BodyFatPercent,
                ActiveGoal = goal == null ? null : ToGoalDto(goal),
                GoalStatusLabel = GoalStatusLabel(latest, goal)
            };
        }).ToList();
    }

    private async Task<List<string>?> ReviewableStudentIds()
    {
        if (User.IsInRole("Admin"))
        {
            return await _context.Students.Select(s => s.StudentId).ToListAsync();
        }

        var trainerId = CurrentTrainerId();
        if (string.IsNullOrEmpty(trainerId))
        {
            return null;
        }

        return await _context.StudentWorkoutAssignments
            .Where(a => a.TrainerId == trainerId)
            .Select(a => a.StudentId)
            .Distinct()
            .ToListAsync();
    }

    private async Task<bool> CanReviewStudent(string studentId)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        var trainerId = CurrentTrainerId();
        return !string.IsNullOrEmpty(trainerId)
            && await _context.StudentWorkoutAssignments.AnyAsync(a => a.TrainerId == trainerId && a.StudentId == studentId);
    }

    private static string? ValidateLog(UpsertProgressLogDto dto)
    {
        if (dto.WeightKg is <= 0 || dto.BodyFatPercent is < 0 or > 100)
        {
            return "PROGRESS_VALUES_INVALID";
        }

        if (dto.WaistCm is < 0 || dto.ChestCm is < 0 || dto.ArmCm is < 0 || dto.HipCm is < 0)
        {
            return "PROGRESS_MEASUREMENTS_INVALID";
        }

        if (dto.WeightKg == null && dto.BodyFatPercent == null && dto.WaistCm == null
            && dto.ChestCm == null && dto.ArmCm == null && dto.HipCm == null
            && string.IsNullOrWhiteSpace(dto.StrengthNotes) && string.IsNullOrWhiteSpace(dto.Notes))
        {
            return "PROGRESS_LOG_EMPTY";
        }

        return null;
    }

    private static string? ValidateGoal(UpsertProgressGoalDto dto)
    {
        if (dto.TargetWeightKg is <= 0 || dto.TargetBodyFatPercent is < 0 or > 100)
        {
            return "PROGRESS_GOAL_VALUES_INVALID";
        }

        return null;
    }

    private static string GoalStatusLabel(StudentProgressLog? latest, StudentProgressGoal? goal)
    {
        if (goal == null)
        {
            return "No goal";
        }

        if (goal.GoalDate.HasValue && goal.GoalDate.Value.Date < DateTime.UtcNow.Date)
        {
            return "Needs review";
        }

        if (latest?.WeightKg != null && goal.TargetWeightKg != null && latest.WeightKg <= goal.TargetWeightKg)
        {
            return "On track";
        }

        if (latest?.BodyFatPercent != null && goal.TargetBodyFatPercent != null && latest.BodyFatPercent <= goal.TargetBodyFatPercent)
        {
            return "On track";
        }

        return latest == null ? "Waiting for check-in" : "Needs review";
    }

    private static ProgressLogDto ToLogDto(StudentProgressLog log)
    {
        return new ProgressLogDto
        {
            ProgressLogId = log.ProgressLogId,
            StudentId = log.StudentId,
            StudentName = log.Student?.Name,
            StudentEmail = log.Student?.Email,
            LoggedAt = log.LoggedAt,
            WeightKg = log.WeightKg,
            BodyFatPercent = log.BodyFatPercent,
            WaistCm = log.WaistCm,
            ChestCm = log.ChestCm,
            ArmCm = log.ArmCm,
            HipCm = log.HipCm,
            StrengthNotes = log.StrengthNotes,
            Notes = log.Notes
        };
    }

    private static ProgressGoalDto ToGoalDto(StudentProgressGoal goal)
    {
        return new ProgressGoalDto
        {
            ProgressGoalId = goal.ProgressGoalId,
            StudentId = goal.StudentId,
            TargetWeightKg = goal.TargetWeightKg,
            TargetBodyFatPercent = goal.TargetBodyFatPercent,
            GoalDate = goal.GoalDate,
            Status = goal.Status,
            Notes = goal.Notes,
            CreatedAt = goal.CreatedAt,
            UpdatedAt = goal.UpdatedAt
        };
    }

    private static string NormalizeGoalStatus(string? status)
    {
        return status?.Trim() switch
        {
            ProgressGoalStatus.Completed => ProgressGoalStatus.Completed,
            ProgressGoalStatus.Cancelled => ProgressGoalStatus.Cancelled,
            _ => ProgressGoalStatus.Active
        };
    }

    private async Task NotifyAssignedTrainers(string studentId, string title, string content, string link)
    {
        var trainerUserIds = await _context.StudentWorkoutAssignments
            .Where(a => a.StudentId == studentId && a.Trainer != null && a.Trainer.UserId != null)
            .Select(a => a.Trainer!.UserId!)
            .Distinct()
            .ToListAsync();

        await _notificationRepository.NotifyUsers(
            trainerUserIds,
            NotificationTypes.Progress,
            title,
            content,
            link);
    }

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }

    private string CurrentTrainerId()
    {
        return User.FindFirstValue("trainerId") ?? string.Empty;
    }
}
