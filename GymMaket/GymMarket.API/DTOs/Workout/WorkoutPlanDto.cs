namespace GymMarket.API.DTOs.Workout;

public class WorkoutPlanDto
{
    public string WorkoutPlanId { get; set; } = string.Empty;

    public string? TrainerId { get; set; }

    public string? TrainerName { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Goal { get; set; }

    public string Difficulty { get; set; } = string.Empty;

    public int DurationWeeks { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<WorkoutExerciseDto> Exercises { get; set; } = new();
}
