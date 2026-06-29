namespace GymMarket.API.Models;

public class StudentWorkoutAssignment
{
    public string AssignmentId { get; set; } = null!;

    public string WorkoutPlanId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string? TrainerId { get; set; }

    public string Status { get; set; } = WorkoutAssignmentStatus.Active;

    public DateTime StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual WorkoutPlan? WorkoutPlan { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Trainer? Trainer { get; set; }

    public virtual ICollection<WorkoutExerciseCompletion> Completions { get; set; } = new List<WorkoutExerciseCompletion>();
}
