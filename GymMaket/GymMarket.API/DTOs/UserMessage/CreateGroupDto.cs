using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    public class CreateGroupDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> MemberIds { get; set; } = new();
    }
}
