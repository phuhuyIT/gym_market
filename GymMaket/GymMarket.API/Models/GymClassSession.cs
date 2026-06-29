namespace GymMarket.API.Models;

public class GymClassSession
{
    public string ClassSessionId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? TrainerId { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public int Capacity { get; set; }

    public string? Location { get; set; }

    public string Status { get; set; } = ClassSessionStatus.Scheduled;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual Trainer? Trainer { get; set; }

    public virtual ICollection<ClassBooking> Bookings { get; set; } = new List<ClassBooking>();
}
