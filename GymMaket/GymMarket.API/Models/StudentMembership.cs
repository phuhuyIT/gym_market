namespace GymMarket.API.Models;

public class StudentMembership
{
    public string MembershipId { get; set; } = null!;

    public string PlanId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string Status { get; set; } = MembershipStatus.Active;

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual MembershipPlan? Plan { get; set; }

    public virtual Student? Student { get; set; }
}
