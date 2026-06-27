namespace GymMarket.API.Models;

public class MembershipPlan
{
    public string PlanId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int DurationDays { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<StudentMembership> StudentMemberships { get; set; } = new List<StudentMembership>();
}
