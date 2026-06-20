using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class ConfirmEmailDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
