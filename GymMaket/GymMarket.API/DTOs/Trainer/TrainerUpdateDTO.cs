using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Trainer
{
    public class TrainerUpdateDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        public string? Password { get; set; }

        [StringLength(200, ErrorMessage = "Certification cannot exceed 200 characters.")]
        public string? Certification { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public int? Experience { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public decimal? Rating { get; set; }

        [Url(ErrorMessage = "ProfilePicture must be a valid URL.")]
        public string? ProfilePicture { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
