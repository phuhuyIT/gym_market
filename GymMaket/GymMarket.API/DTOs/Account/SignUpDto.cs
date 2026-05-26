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
    }
}
