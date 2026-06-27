namespace GymMarket.API.DTOs.Admin
{
    public class AdminUserListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = "Active";
        public bool EmailConfirmed { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = [];
        public string? StudentId { get; set; }
        public string? TrainerId { get; set; }
        public string? TrainerApprovalStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
