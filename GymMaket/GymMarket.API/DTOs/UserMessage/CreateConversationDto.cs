using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    public class CreateConversationDto
    {
        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string RecieveId { get; set; } = string.Empty;
    }
}
