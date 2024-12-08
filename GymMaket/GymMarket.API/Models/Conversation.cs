using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string RecieveId { get; set; } = string.Empty;
        public AppUser? Recieve { get; set; }
    }
}
