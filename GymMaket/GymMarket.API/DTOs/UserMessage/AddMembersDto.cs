using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    public class AddMembersDto
    {
        [Required]
        public int ConversationId { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> UserIds { get; set; } = new();
    }
}
