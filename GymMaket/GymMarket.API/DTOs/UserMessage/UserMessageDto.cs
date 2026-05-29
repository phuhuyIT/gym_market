namespace GymMarket.API.DTOs.UserMessage
{
    public class UserMessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }

        public string Content { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
