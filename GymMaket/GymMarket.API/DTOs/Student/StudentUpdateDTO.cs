using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Student
{
    public class StudentUpdateDTO
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

        [StringLength(200, ErrorMessage = "HealthStatus cannot exceed 200 characters.")]
        public string? HealthStatus { get; set; }

        [Url(ErrorMessage = "ProfilePicture must be a valid URL.")]
        public string? ProfilePicture { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
