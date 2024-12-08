using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    public class SendMessageDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int ConversationId { get; set; }

        [Required]
        public string SenderId { get; set;} = string.Empty;

        public string Avatar { get; set; } = string.Empty;
    }
}
