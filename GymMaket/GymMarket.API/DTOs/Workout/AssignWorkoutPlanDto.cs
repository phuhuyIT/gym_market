namespace GymMarket.API.DTOs.Workout;

public class AssignWorkoutPlanDto
{
    public string StudentId { get; set; } = string.Empty;

    public DateTime? StartsAt { get; set; }
}
