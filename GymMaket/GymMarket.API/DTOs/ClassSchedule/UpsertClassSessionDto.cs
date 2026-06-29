namespace GymMarket.API.DTOs.ClassSchedule;

public class UpsertClassSessionDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? TrainerId { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public int Capacity { get; set; }

    public string? Location { get; set; }

    public string Status { get; set; } = "Scheduled";
}
