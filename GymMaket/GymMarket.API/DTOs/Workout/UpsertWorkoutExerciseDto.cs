namespace GymMarket.API.DTOs.Workout;

public class UpsertWorkoutExerciseDto
{
    public int WeekNumber { get; set; } = 1;

    public int DayNumber { get; set; } = 1;

    public int Order { get; set; } = 1;

    public string Name { get; set; } = string.Empty;

    public int Sets { get; set; } = 3;

    public string Reps { get; set; } = string.Empty;

    public int RestSeconds { get; set; } = 60;

    public string? Notes { get; set; }
}
