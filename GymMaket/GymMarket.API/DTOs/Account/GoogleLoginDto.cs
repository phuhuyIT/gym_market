using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class GoogleLoginDto
    {
        [Required]
        public string IdToken { get; set; } = null!;

        public string? Role { get; set; }
    }
}
