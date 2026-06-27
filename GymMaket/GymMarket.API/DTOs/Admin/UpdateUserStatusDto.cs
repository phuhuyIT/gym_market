using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Admin
{
    public class UpdateUserStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
