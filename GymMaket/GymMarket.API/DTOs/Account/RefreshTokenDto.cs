using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
