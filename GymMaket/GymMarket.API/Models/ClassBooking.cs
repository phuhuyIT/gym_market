namespace GymMarket.API.Models;

public class ClassBooking
{
    public string BookingId { get; set; } = null!;

    public string ClassSessionId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string Status { get; set; } = ClassBookingStatus.Booked;

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CancelledAt { get; set; }

    public DateTime? AttendanceMarkedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual GymClassSession? ClassSession { get; set; }

    public virtual Student? Student { get; set; }
}
