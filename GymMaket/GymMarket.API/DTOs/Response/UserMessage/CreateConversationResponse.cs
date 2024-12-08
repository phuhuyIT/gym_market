using GymMarket.API.DTOs.UserMessage;

namespace GymMarket.API.DTOs.Response.UserMessage
{
    public class CreateConversationResponse : ApiResponse
    {
        public ConversitionDto? ConversitionDto { get; set; }
    }
}
