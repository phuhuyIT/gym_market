namespace GymMarket.API.DTOs.Membership;

public class MembershipStatusDto
{
    public bool HasActiveMembership { get; set; }

    public StudentMembershipDto? CurrentMembership { get; set; }

    public List<MembershipPlanDto> AvailablePlans { get; set; } = [];
}
