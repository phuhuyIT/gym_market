using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    public class UpdateMemberRoleDto
    {
        [Required]
        public int ConversationId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
