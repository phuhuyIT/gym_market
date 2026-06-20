using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.User
{
    // The user being updated is always the authenticated caller (derived from the
    // JWT in the controller), so the DTO carries no user id.
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Avatar is required")]
        public string Avatar { get; set; } = string.Empty;

        public string? Status { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
