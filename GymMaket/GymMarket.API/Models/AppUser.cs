using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        // link to Student and Trainer
        public virtual Student? Student { get; set; }
        public virtual Trainer? Trainer { get; set; }
    }
}
