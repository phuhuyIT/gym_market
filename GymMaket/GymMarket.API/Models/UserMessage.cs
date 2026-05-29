using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.Models
{
    public class UserMessage
    {
        [Key]
        public int Id { get; set; }
        
        public string Content { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;
        public int ConversationId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Type { get; set; } = MessageTypes.Text;

        public AppUser? AppUser { get; set; }
    }
}
