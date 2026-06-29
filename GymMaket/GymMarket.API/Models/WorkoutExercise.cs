namespace GymMarket.API.Models;

public class WorkoutExercise
{
    public string ExerciseId { get; set; } = null!;

    public string WorkoutPlanId { get; set; } = null!;

    public int WeekNumber { get; set; }

    public int DayNumber { get; set; }

    public int Order { get; set; }

    public string Name { get; set; } = null!;

    public int Sets { get; set; }

    public string Reps { get; set; } = string.Empty;

    public int RestSeconds { get; set; }

    public string? Notes { get; set; }

    public virtual WorkoutPlan? WorkoutPlan { get; set; }

    public virtual ICollection<WorkoutExerciseCompletion> Completions { get; set; } = new List<WorkoutExerciseCompletion>();
}
