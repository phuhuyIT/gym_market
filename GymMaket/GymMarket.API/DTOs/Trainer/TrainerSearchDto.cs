namespace GymMarket.API.DTOs.Trainer
{
    public class TrainerSearchDto
    {
        public string TrainerId { get; set; } = null!;
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Certification { get; set; }
        public string? Category { get; set; }
        public string? Bio { get; set; }
        public int? Experience { get; set; }
        public decimal? Rating { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
