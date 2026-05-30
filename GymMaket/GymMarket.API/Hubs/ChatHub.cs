using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GymMarket.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IPresenceTracker _presenceTracker;

        public ChatHub(IConversationRepository conversationRepository, IPresenceTracker presenceTracker)
        {
            _conversationRepository = conversationRepository;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var conversationIds = await _conversationRepository.GetConversationIdsOfUser(userId);
                foreach (var conversationId in conversationIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
                }

                var cameOnline = _presenceTracker.UserConnected(userId, Context.ConnectionId);
                if (cameOnline)
                {
                    await NotifyConversations(conversationIds, "UserOnline", userId);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var wentOffline = _presenceTracker.UserDisconnected(userId, Context.ConnectionId);
                if (wentOffline)
                {
                    var lastSeen = DateTime.Now;
                    await _conversationRepository.UpdateLastSeen(userId, lastSeen);

                    var conversationIds = await _conversationRepository.GetConversationIdsOfUser(userId);
                    await NotifyConversations(conversationIds, "UserOffline", userId, lastSeen);
                }
            }
            await base.OnDisconnectedAsync(exception);
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
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User is not authenticated.");
            }

            message.SenderId = userId;
            var saved = await _conversationRepository.SendMessage(message);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", saved);
        }

        private async Task NotifyConversations(List<int> conversationIds, string eventName, params object?[] args)
        {
            foreach (var conversationId in conversationIds)
            {
                await Clients.Group(conversationId.ToString()).SendCoreAsync(eventName, args);
            }
        }
    }
}
