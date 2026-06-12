namespace GymMarket.API.DTOs.UserMessage
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public string ConversationName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime? LastMessageAt { get; set; }
        public bool HasNewMessage { get; set; }

        public string Avatar { get; set; } = string.Empty;

        public bool IsGroup { get; set; }
        public string Role { get; set; } = string.Empty;
        public int MemberCount { get; set; }

        // Presence (for direct chats, this reflects the other participant).
        public string? OtherUserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
