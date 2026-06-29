namespace GymMarket.API.Models;

public class WorkoutExerciseCompletion
{
    public string CompletionId { get; set; } = null!;

    public string AssignmentId { get; set; } = null!;

    public string ExerciseId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public string? Notes { get; set; }

    public virtual StudentWorkoutAssignment? Assignment { get; set; }

    public virtual WorkoutExercise? Exercise { get; set; }

    public virtual Student? Student { get; set; }
}
