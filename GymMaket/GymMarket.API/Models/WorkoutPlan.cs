namespace GymMarket.API.Models;

public class WorkoutPlan
{
    public string WorkoutPlanId { get; set; } = null!;

    public string? TrainerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Goal { get; set; }

    public string Difficulty { get; set; } = "Beginner";

    public int DurationWeeks { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual Trainer? Trainer { get; set; }

    public virtual ICollection<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();

    public virtual ICollection<StudentWorkoutAssignment> Assignments { get; set; } = new List<StudentWorkoutAssignment>();
}
