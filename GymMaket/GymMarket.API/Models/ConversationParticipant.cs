using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.Models
{
    public class ConversationParticipant
    {
        public int Id { get; set; }

        public string LastMessage { get; set; } = string.Empty;
        public bool HasNewMessage { get; set; }


        public int ConversationId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
