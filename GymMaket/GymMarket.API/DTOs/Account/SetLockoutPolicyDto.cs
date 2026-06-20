using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Account
{
    public class SetLockoutPolicyDto
    {
        [Required]
        [Range(1, 20)]
        public int MaxFailedAccessAttempts { get; set; }

        [Required]
        [Range(1, 1440)]
        public int LockoutDurationInMinutes { get; set; }
    }
}
