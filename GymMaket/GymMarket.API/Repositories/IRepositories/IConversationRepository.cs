using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.UserMessage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IConversationRepository
    {
        Task<ApiResponse> CreateConversation(CreateConversationDto model);
        Task<List<ConversitionDto>> GetConversationOfUser(string userId);
        Task SeenMessage(string userId, int conversationId);
        Task SendMessage(SendMessageDto model);
        Task<List<UserMessageDto>> GetMessages(int conversationId);
    }
}
