using GymMarket.API.Models;
using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.UserMessage
{
    public class UserMessageDto
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;

        public AppUser? AppUser { get; set; }
    }
}
