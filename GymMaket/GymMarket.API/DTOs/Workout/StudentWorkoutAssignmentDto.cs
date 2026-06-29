namespace GymMarket.API.DTOs.Workout;

public class StudentWorkoutAssignmentDto
{
    public string AssignmentId { get; set; } = string.Empty;

    public string WorkoutPlanId { get; set; } = string.Empty;

    public string PlanName { get; set; } = string.Empty;

    public string? Goal { get; set; }

    public string Difficulty { get; set; } = string.Empty;

    public int DurationWeeks { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public string? StudentName { get; set; }

    public string? StudentEmail { get; set; }

    public string? TrainerId { get; set; }

    public string? TrainerName { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public int TotalExercises { get; set; }

    public int CompletedExercises { get; set; }

    public decimal CompletionPercent { get; set; }

    public List<WorkoutExerciseDto> Exercises { get; set; } = new();
}
