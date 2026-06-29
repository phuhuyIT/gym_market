namespace GymMarket.API.DTOs.Workout;

public class UpsertWorkoutPlanDto
{
    public string? TrainerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Goal { get; set; }

    public string Difficulty { get; set; } = "Beginner";

    public int DurationWeeks { get; set; } = 4;

    public bool IsActive { get; set; } = true;

    public List<UpsertWorkoutExerciseDto> Exercises { get; set; } = new();
}
