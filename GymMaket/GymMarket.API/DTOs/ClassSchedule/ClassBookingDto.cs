namespace GymMarket.API.DTOs.ClassSchedule;

public class ClassBookingDto
{
    public string BookingId { get; set; } = string.Empty;

    public string ClassSessionId { get; set; } = string.Empty;

    public string ClassTitle { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string? StudentName { get; set; }

    public string? StudentEmail { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime BookedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime? AttendanceMarkedAt { get; set; }
}
