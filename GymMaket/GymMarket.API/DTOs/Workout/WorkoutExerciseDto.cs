namespace GymMarket.API.DTOs.Workout;

public class WorkoutExerciseDto
{
    public string ExerciseId { get; set; } = string.Empty;

    public int WeekNumber { get; set; }

    public int DayNumber { get; set; }

    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Sets { get; set; }

    public string Reps { get; set; } = string.Empty;

    public int RestSeconds { get; set; }

    public string? Notes { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }
}
