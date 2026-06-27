namespace GymMarket.API.DTOs.Membership;

public class StudentMembershipDto
{
    public string MembershipId { get; set; } = string.Empty;

    public string PlanId { get; set; } = string.Empty;

    public string PlanName { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string? StudentName { get; set; }

    public string? StudentEmail { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public decimal Price { get; set; }

    public int DurationDays { get; set; }
}
