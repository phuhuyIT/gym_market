using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GymMarket.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversationRepository conversationRepository;

        public ChatHub(IConversationRepository conversationRepository)
        {
            this.conversationRepository = conversationRepository;
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendMessageToRoom(string roomName, SendMessageDto message)
        {
            message.SenderId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await conversationRepository.SendMessage(message);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", message);
        }
    }
}
