namespace GymMarket.API.DTOs.UserMessage
{
    public class GroupMemberDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
