namespace GymMarket.API.DTOs.User
{
    public class GetUserInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
