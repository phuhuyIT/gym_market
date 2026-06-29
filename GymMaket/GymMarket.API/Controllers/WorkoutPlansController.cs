using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Workout;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WorkoutPlansController : ControllerBase
{
    private readonly GymMarketContext _context;

    public WorkoutPlansController(GymMarketContext context)
    {
        _context = context;
    }

    [HttpGet("plans")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<List<WorkoutPlanDto>>> GetPlans([FromQuery] bool includeInactive = false)
    {
        var query = _context.WorkoutPlans
            .Include(p => p.Trainer)
            .Include(p => p.Exercises)
            .AsQueryable();

        if (User.IsInRole("Trainer"))
        {
            var trainerId = CurrentTrainerId();
            if (string.IsNullOrEmpty(trainerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_TRAINER" });
            }

            query = query.Where(p => p.TrainerId == trainerId);
        }

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        var plans = await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(200)
            .ToListAsync();

        return Ok(plans.Select(ToPlanDto).ToList());
    }

    [HttpPost("plans")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> CreatePlan(UpsertWorkoutPlanDto dto)
    {
        var validation = await ValidatePlan(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var trainerId = await ResolveTrainerId(dto.TrainerId);
        if (trainerId == TrainerAccess.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "WORKOUT_PLAN_TRAINER_FORBIDDEN" });
        }

        var plan = new WorkoutPlan
        {
            WorkoutPlanId = Guid.NewGuid().ToString(),
            TrainerId = trainerId,
            Name = dto.Name.Trim(),
            Goal = string.IsNullOrWhiteSpace(dto.Goal) ? null : dto.Goal.Trim(),
            Difficulty = NormalizeDifficulty(dto.Difficulty),
            DurationWeeks = dto.DurationWeeks,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            Exercises = BuildExercises(dto.Exercises)
        };

        _context.WorkoutPlans.Add(plan);
        await _context.SaveChangesAsync();

        plan.Trainer = string.IsNullOrEmpty(plan.TrainerId)
            ? null
            : await _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == plan.TrainerId);

        return Ok(ToPlanDto(plan));
    }

    [HttpPut("plans/{workoutPlanId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<WorkoutPlanDto>> UpdatePlan(string workoutPlanId, UpsertWorkoutPlanDto dto)
    {
        var plan = await _context.WorkoutPlans
            .Include(p => p.Trainer)
            .Include(p => p.Exercises)
            .Include(p => p.Assignments)
            .FirstOrDefaultAsync(p => p.WorkoutPlanId == workoutPlanId);

        if (plan == null)
        {
            return NotFound(new { Message = "WORKOUT_PLAN_NOT_FOUND" });
        }

        if (!CanManagePlan(plan))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "WORKOUT_PLAN_FORBIDDEN" });
        }

        if (plan.Assignments.Count > 0)
        {
            return Conflict(new { Message = "WORKOUT_PLAN_HAS_ASSIGNMENTS" });
        }

        var validation = await ValidatePlan(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var trainerId = await ResolveTrainerId(dto.TrainerId);
        if (trainerId == TrainerAccess.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "WORKOUT_PLAN_TRAINER_FORBIDDEN" });
        }

        plan.TrainerId = trainerId;
        plan.Name = dto.Name.Trim();
        plan.Goal = string.IsNullOrWhiteSpace(dto.Goal) ? null : dto.Goal.Trim();
        plan.Difficulty = NormalizeDifficulty(dto.Difficulty);
        plan.DurationWeeks = dto.DurationWeeks;
        plan.IsActive = dto.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        _context.WorkoutExercises.RemoveRange(plan.Exercises);
        plan.Exercises = BuildExercises(dto.Exercises);
        await _context.SaveChangesAsync();

        plan.Trainer = string.IsNullOrEmpty(plan.TrainerId)
            ? null
            : await _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == plan.TrainerId);

        return Ok(ToPlanDto(plan));
    }

    [HttpDelete("plans/{workoutPlanId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> DeactivatePlan(string workoutPlanId)
    {
        var plan = await _context.WorkoutPlans.FirstOrDefaultAsync(p => p.WorkoutPlanId == workoutPlanId);
        if (plan == null)
        {
            return NotFound(new { Message = "WORKOUT_PLAN_NOT_FOUND" });
        }

        if (!CanManagePlan(plan))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "WORKOUT_PLAN_FORBIDDEN" });
        }

        plan.IsActive = false;
        plan.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "WORKOUT_PLAN_DEACTIVATED" });
    }

    [HttpPost("plans/{workoutPlanId}/assign")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<StudentWorkoutAssignmentDto>> AssignPlan(string workoutPlanId, AssignWorkoutPlanDto dto)
    {
        var plan = await _context.WorkoutPlans
            .Include(p => p.Trainer)
            .Include(p => p.Exercises)
            .FirstOrDefaultAsync(p => p.WorkoutPlanId == workoutPlanId);

        if (plan == null || !plan.IsActive)
        {
            return NotFound(new { Message = "WORKOUT_PLAN_NOT_FOUND" });
        }

        if (!CanManagePlan(plan))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "WORKOUT_PLAN_FORBIDDEN" });
        }

        if (plan.Exercises.Count == 0)
        {
            return Conflict(new { Message = "WORKOUT_PLAN_HAS_NO_EXERCISES" });
        }

        if (string.IsNullOrWhiteSpace(dto.StudentId))
        {
            return BadRequest(new { Message = "STUDENT_REQUIRED" });
        }

        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == dto.StudentId.Trim());
        if (student == null)
        {
            return NotFound(new { Message = "STUDENT_NOT_FOUND" });
        }

        var hasActiveAssignment = await _context.StudentWorkoutAssignments.AnyAsync(a =>
            a.StudentId == student.StudentId
            && a.WorkoutPlanId == plan.WorkoutPlanId
            && a.Status == WorkoutAssignmentStatus.Active);
        if (hasActiveAssignment)
        {
            return Conflict(new { Message = "WORKOUT_PLAN_ALREADY_ASSIGNED" });
        }

        var startsAt = (dto.StartsAt ?? DateTime.UtcNow).ToUniversalTime();
        if (startsAt < DateTime.UtcNow.Date)
        {
            startsAt = DateTime.UtcNow;
        }

        var assignment = new StudentWorkoutAssignment
        {
            AssignmentId = Guid.NewGuid().ToString(),
            WorkoutPlanId = plan.WorkoutPlanId,
            StudentId = student.StudentId,
            TrainerId = plan.TrainerId,
            Status = WorkoutAssignmentStatus.Active,
            StartsAt = startsAt,
            EndsAt = startsAt.AddDays(plan.DurationWeeks * 7),
            CreatedAt = DateTime.UtcNow,
            WorkoutPlan = plan,
            Student = student,
            Trainer = plan.Trainer
        };

        _context.StudentWorkoutAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return Ok(ToAssignmentDto(assignment));
    }

    [HttpGet("assignments")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<List<StudentWorkoutAssignmentDto>>> GetAssignments([FromQuery] string status = "")
    {
        var query = AssignmentQuery();

        if (User.IsInRole("Trainer"))
        {
            var trainerId = CurrentTrainerId();
            if (string.IsNullOrEmpty(trainerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_TRAINER" });
            }

            query = query.Where(a => a.TrainerId == trainerId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status.Trim());
        }

        var assignments = await query
            .OrderByDescending(a => a.CreatedAt)
            .Take(200)
            .ToListAsync();

        return Ok(assignments.Select(ToAssignmentDto).ToList());
    }

    [HttpGet("my-assignments")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<List<StudentWorkoutAssignmentDto>>> GetMyAssignments([FromQuery] string status = "")
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var query = AssignmentQuery()
            .Where(a => a.StudentId == studentId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status.Trim());
        }

        var assignments = await query
            .OrderByDescending(a => a.CreatedAt)
            .Take(100)
            .ToListAsync();

        return Ok(assignments.Select(ToAssignmentDto).ToList());
    }

    [HttpPost("assignments/{assignmentId}/exercises/{exerciseId}/complete")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentWorkoutAssignmentDto>> CompleteExercise(
        string assignmentId,
        string exerciseId,
        CompleteWorkoutExerciseDto dto)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var assignment = await AssignmentQuery()
            .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId && a.StudentId == studentId);

        if (assignment == null)
        {
            return NotFound(new { Message = "WORKOUT_ASSIGNMENT_NOT_FOUND" });
        }

        if (assignment.Status == WorkoutAssignmentStatus.Cancelled)
        {
            return Conflict(new { Message = "WORKOUT_ASSIGNMENT_CANCELLED" });
        }

        var exercise = assignment.WorkoutPlan?.Exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);
        if (exercise == null)
        {
            return NotFound(new { Message = "WORKOUT_EXERCISE_NOT_FOUND" });
        }

        var completion = assignment.Completions.FirstOrDefault(c => c.ExerciseId == exerciseId);
        if (completion == null)
        {
            completion = new WorkoutExerciseCompletion
            {
                CompletionId = Guid.NewGuid().ToString(),
                AssignmentId = assignment.AssignmentId,
                ExerciseId = exercise.ExerciseId,
                StudentId = studentId,
                CompletedAt = DateTime.UtcNow,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
            };
            _context.WorkoutExerciseCompletions.Add(completion);
            assignment.Completions.Add(completion);
        }
        else if (!string.IsNullOrWhiteSpace(dto.Notes))
        {
            completion.Notes = dto.Notes.Trim();
        }

        var totalExercises = assignment.WorkoutPlan?.Exercises.Count ?? 0;
        var completedExercises = assignment.Completions.Select(c => c.ExerciseId).Distinct().Count();
        if (totalExercises > 0 && completedExercises >= totalExercises)
        {
            assignment.Status = WorkoutAssignmentStatus.Completed;
            assignment.CompletedAt ??= DateTime.UtcNow;
        }

        assignment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        assignment = await AssignmentQuery().FirstAsync(a => a.AssignmentId == assignmentId);
        return Ok(ToAssignmentDto(assignment));
    }

    [HttpPost("assignments/{assignmentId}/cancel")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> CancelAssignment(string assignmentId)
    {
        var assignment = await _context.StudentWorkoutAssignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        if (assignment == null)
        {
            return NotFound(new { Message = "WORKOUT_ASSIGNMENT_NOT_FOUND" });
        }

        if (!CanManageAssignment(assignment))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "WORKOUT_ASSIGNMENT_FORBIDDEN" });
        }

        assignment.Status = WorkoutAssignmentStatus.Cancelled;
        assignment.CancelledAt = DateTime.UtcNow;
        assignment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "WORKOUT_ASSIGNMENT_CANCELLED" });
    }

    private IQueryable<StudentWorkoutAssignment> AssignmentQuery()
    {
        return _context.StudentWorkoutAssignments
            .Include(a => a.Student)
            .Include(a => a.Trainer)
            .Include(a => a.WorkoutPlan)
                .ThenInclude(p => p!.Trainer)
            .Include(a => a.WorkoutPlan)
                .ThenInclude(p => p!.Exercises)
            .Include(a => a.Completions)
            .AsQueryable();
    }

    private async Task<string?> ValidatePlan(UpsertWorkoutPlanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return "WORKOUT_PLAN_NAME_REQUIRED";
        }

        if (dto.DurationWeeks <= 0)
        {
            return "WORKOUT_PLAN_DURATION_INVALID";
        }

        if (dto.Exercises.Count == 0)
        {
            return "WORKOUT_EXERCISES_REQUIRED";
        }

        foreach (var exercise in dto.Exercises)
        {
            if (string.IsNullOrWhiteSpace(exercise.Name))
            {
                return "WORKOUT_EXERCISE_NAME_REQUIRED";
            }

            if (exercise.WeekNumber <= 0 || exercise.DayNumber <= 0 || exercise.Order <= 0)
            {
                return "WORKOUT_EXERCISE_POSITION_INVALID";
            }

            if (exercise.Sets <= 0 || exercise.RestSeconds < 0)
            {
                return "WORKOUT_EXERCISE_VOLUME_INVALID";
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.TrainerId)
            && !await _context.Trainers.AnyAsync(t => t.TrainerId == dto.TrainerId.Trim()))
        {
            return "TRAINER_NOT_FOUND";
        }

        return null;
    }

    private async Task<string?> ResolveTrainerId(string? requestedTrainerId)
    {
        if (User.IsInRole("Trainer"))
        {
            var currentTrainerId = CurrentTrainerId();
            if (string.IsNullOrEmpty(currentTrainerId))
            {
                return TrainerAccess.Forbidden;
            }

            if (!string.IsNullOrWhiteSpace(requestedTrainerId) && requestedTrainerId.Trim() != currentTrainerId)
            {
                return TrainerAccess.Forbidden;
            }

            return currentTrainerId;
        }

        var trainerId = string.IsNullOrWhiteSpace(requestedTrainerId) ? null : requestedTrainerId.Trim();
        if (!string.IsNullOrEmpty(trainerId) && !await _context.Trainers.AnyAsync(t => t.TrainerId == trainerId))
        {
            return TrainerAccess.Forbidden;
        }

        return trainerId;
    }

    private bool CanManagePlan(WorkoutPlan plan)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        var trainerId = CurrentTrainerId();
        return !string.IsNullOrEmpty(trainerId) && plan.TrainerId == trainerId;
    }

    private bool CanManageAssignment(StudentWorkoutAssignment assignment)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        var trainerId = CurrentTrainerId();
        return !string.IsNullOrEmpty(trainerId) && assignment.TrainerId == trainerId;
    }

    private static List<WorkoutExercise> BuildExercises(IEnumerable<UpsertWorkoutExerciseDto> exerciseDtos)
    {
        return exerciseDtos
            .OrderBy(e => e.WeekNumber)
            .ThenBy(e => e.DayNumber)
            .ThenBy(e => e.Order)
            .Select(e => new WorkoutExercise
            {
                ExerciseId = Guid.NewGuid().ToString(),
                WeekNumber = e.WeekNumber,
                DayNumber = e.DayNumber,
                Order = e.Order,
                Name = e.Name.Trim(),
                Sets = e.Sets,
                Reps = e.Reps.Trim(),
                RestSeconds = e.RestSeconds,
                Notes = string.IsNullOrWhiteSpace(e.Notes) ? null : e.Notes.Trim()
            })
            .ToList();
    }

    private static WorkoutPlanDto ToPlanDto(WorkoutPlan plan)
    {
        return new WorkoutPlanDto
        {
            WorkoutPlanId = plan.WorkoutPlanId,
            TrainerId = plan.TrainerId,
            TrainerName = plan.Trainer?.Name,
            Name = plan.Name,
            Goal = plan.Goal,
            Difficulty = plan.Difficulty,
            DurationWeeks = plan.DurationWeeks,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            Exercises = plan.Exercises
                .OrderBy(e => e.WeekNumber)
                .ThenBy(e => e.DayNumber)
                .ThenBy(e => e.Order)
                .Select(e => ToExerciseDto(e, null))
                .ToList()
        };
    }

    private static StudentWorkoutAssignmentDto ToAssignmentDto(StudentWorkoutAssignment assignment)
    {
        var plan = assignment.WorkoutPlan;
        var exercises = plan?.Exercises
            .OrderBy(e => e.WeekNumber)
            .ThenBy(e => e.DayNumber)
            .ThenBy(e => e.Order)
            .Select(e => ToExerciseDto(e, assignment.Completions.FirstOrDefault(c => c.ExerciseId == e.ExerciseId)))
            .ToList() ?? new List<WorkoutExerciseDto>();
        var completed = assignment.Completions.Select(c => c.ExerciseId).Distinct().Count();
        var total = exercises.Count;

        return new StudentWorkoutAssignmentDto
        {
            AssignmentId = assignment.AssignmentId,
            WorkoutPlanId = assignment.WorkoutPlanId,
            PlanName = plan?.Name ?? string.Empty,
            Goal = plan?.Goal,
            Difficulty = plan?.Difficulty ?? string.Empty,
            DurationWeeks = plan?.DurationWeeks ?? 0,
            StudentId = assignment.StudentId,
            StudentName = assignment.Student?.Name,
            StudentEmail = assignment.Student?.Email,
            TrainerId = assignment.TrainerId,
            TrainerName = assignment.Trainer?.Name ?? plan?.Trainer?.Name,
            Status = assignment.Status,
            StartsAt = assignment.StartsAt,
            EndsAt = assignment.EndsAt,
            CreatedAt = assignment.CreatedAt,
            CompletedAt = assignment.CompletedAt,
            CancelledAt = assignment.CancelledAt,
            TotalExercises = total,
            CompletedExercises = completed,
            CompletionPercent = total == 0 ? 0 : Math.Round((decimal)completed / total * 100, 2),
            Exercises = exercises
        };
    }

    private static WorkoutExerciseDto ToExerciseDto(WorkoutExercise exercise, WorkoutExerciseCompletion? completion)
    {
        return new WorkoutExerciseDto
        {
            ExerciseId = exercise.ExerciseId,
            WeekNumber = exercise.WeekNumber,
            DayNumber = exercise.DayNumber,
            Order = exercise.Order,
            Name = exercise.Name,
            Sets = exercise.Sets,
            Reps = exercise.Reps,
            RestSeconds = exercise.RestSeconds,
            Notes = exercise.Notes,
            IsCompleted = completion != null,
            CompletedAt = completion?.CompletedAt
        };
    }

    private static string NormalizeDifficulty(string? difficulty)
    {
        return difficulty?.Trim() switch
        {
            "Intermediate" => "Intermediate",
            "Advanced" => "Advanced",
            _ => "Beginner"
        };
    }

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }

    private string CurrentTrainerId()
    {
        return User.FindFirstValue("trainerId") ?? string.Empty;
    }

    private static class TrainerAccess
    {
        public const string Forbidden = "__FORBIDDEN__";
    }
}
