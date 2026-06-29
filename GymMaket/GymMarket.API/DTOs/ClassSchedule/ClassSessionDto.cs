namespace GymMarket.API.DTOs.ClassSchedule;

public class ClassSessionDto
{
    public string ClassSessionId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? TrainerId { get; set; }

    public string? TrainerName { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public int Capacity { get; set; }

    public int BookedCount { get; set; }

    public int AvailableSpots { get; set; }

    public string? Location { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool IsBooked { get; set; }

    public string? MyBookingId { get; set; }

    public string? MyBookingStatus { get; set; }
}
