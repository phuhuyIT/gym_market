using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.User
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; } = string.Empty;

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
