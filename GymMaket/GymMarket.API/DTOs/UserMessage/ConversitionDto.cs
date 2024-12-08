namespace GymMarket.API.DTOs.UserMessage
{
    public class ConversitionDto
    {
        public int ConversationId { get; set; }
        public string ConversationName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public bool HasNewMessage { get; set; }

        public string Avatar { get; set; } = string.Empty;
    }
}
