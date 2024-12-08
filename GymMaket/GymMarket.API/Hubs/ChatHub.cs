using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace GymMarket.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ConversationRepository conversationRepository;

        public ChatHub(ConversationRepository conversationRepository)
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
            await conversationRepository.SendMessage(message);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", message);
        }
    }
}
