using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class SignUpDto
    {
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Password")]
        [StringLength(maximumLength: 16, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "HealthStatus cannot exceed 200 characters.")]
        public string? HealthStatus { get; set; }

        [StringLength(200, ErrorMessage = "Certification cannot exceed 200 characters.")]
        public string? Certification { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string? Category { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public int? Experience { get; set; }
    }
}
