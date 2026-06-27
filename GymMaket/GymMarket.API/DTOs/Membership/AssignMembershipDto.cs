namespace GymMarket.API.DTOs.Membership;

public class AssignMembershipDto
{
    public string PlanId { get; set; } = string.Empty;

    public DateTime? StartsAt { get; set; }
}
