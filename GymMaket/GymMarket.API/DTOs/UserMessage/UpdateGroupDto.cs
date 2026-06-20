using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    public class UpdateGroupDto
    {
        [Required]
        public int ConversationId { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
