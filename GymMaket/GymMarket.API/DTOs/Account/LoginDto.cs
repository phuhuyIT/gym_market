using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
