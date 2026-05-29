using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsGroup { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CreatedById { get; set; }

        // Legacy 1-to-1 fields (used only for direct-conversation dedup).
        public string SenderId { get; set; } = string.Empty;
        public string RecieveId { get; set; } = string.Empty;
        public AppUser? Recieve { get; set; }
    }
}
