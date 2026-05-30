using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.UserMessage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IConversationRepository
    {
        Task<ApiResponse> CreateConversation(CreateConversationDto model);
        Task<ApiResponse> CreateGroup(CreateGroupDto model, string creatorId);
        Task<ApiResponse> AddMembers(AddMembersDto model, string actorId);
        Task<ApiResponse> RemoveMember(int conversationId, string userId, string actorId);
        Task<ApiResponse> LeaveGroup(int conversationId, string userId);
        Task<ApiResponse> UpdateMemberRole(UpdateMemberRoleDto model, string actorId);
        Task<List<GroupMemberDto>> GetGroupMembers(int conversationId);
        Task<List<UserSearchDto>> SearchUsers(string query, string currentUserId);
        Task<List<ConversationDto>> GetConversationOfUser(string userId);
        Task SeenMessage(string userId, int conversationId);
        Task<UserMessageDto?> SendMessage(SendMessageDto model);
        Task<List<UserMessageDto>> GetMessages(int conversationId);
        Task<List<int>> GetConversationIdsOfUser(string userId);
        Task UpdateLastSeen(string userId, DateTime lastSeen);
    }
}
