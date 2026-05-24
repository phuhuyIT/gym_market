namespace GymMarket.API.DTOs.Student
{
    public class StudentProfileDto
    {
        public string StudentId { get; set; } = null!;
        public string? UserId { get; set; }
        public string? ProfilePicture { get; set; }
        public string? HealthStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
    }
}
