using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.User
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Id is requrired")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "FullName is requrired")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is requrired")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Avatar is requrired")]
        public string Avatar { get; set; } = string.Empty;

        public string? Status { get; set; }

        [Required(ErrorMessage = "PhoneNumber is requrired")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
