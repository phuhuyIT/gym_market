using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    // The sender is always the authenticated caller (derived from the JWT in the
    // controller), so the DTO only carries the other participant.
    public class CreateConversationDto
    {
        [Required]
        public string RecieveId { get; set; } = string.Empty;
    }
}
