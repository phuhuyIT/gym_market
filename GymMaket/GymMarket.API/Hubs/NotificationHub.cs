using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GymMarket.API.Hubs
{
    // Server -> client push channel for notifications. Clients never invoke
    // methods on it; the repository pushes "ReceiveNotification" to
    // Clients.User(userId), which SignalR resolves from the JWT's
    // NameIdentifier claim (same claim the ChatHub uses).
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
