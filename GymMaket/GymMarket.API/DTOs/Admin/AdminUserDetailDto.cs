namespace GymMarket.API.DTOs.Admin
{
    public class AdminUserDetailDto : AdminUserListItemDto
    {
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public int AccessFailedCount { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
