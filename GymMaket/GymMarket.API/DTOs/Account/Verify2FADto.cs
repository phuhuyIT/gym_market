using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class Verify2FADto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
