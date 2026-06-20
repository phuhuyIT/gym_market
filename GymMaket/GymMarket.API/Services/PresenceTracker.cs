using System.Collections.Generic;
using System.Linq;

namespace GymMarket.API.Services
{
    public interface IPresenceTracker
    {
        /// <summary>Registers a connection. Returns true if the user just came online (first connection).</summary>
        bool UserConnected(string userId, string connectionId);

        /// <summary>Removes a connection. Returns true if the user just went offline (no connections left).</summary>
        bool UserDisconnected(string userId, string connectionId);

        bool IsOnline(string userId);

        IReadOnlyCollection<string> GetOnlineUsers();
    }

    /// <summary>
    /// In-memory, thread-safe presence registry. Registered as a singleton so the
    /// hub and repositories share the same view of who is currently connected.
    /// A user may hold several connections (multiple tabs/devices); they are only
    /// considered offline once the last connection drops.
    /// </summary>
    public class PresenceTracker : IPresenceTracker
    {
        private readonly Dictionary<string, HashSet<string>> _onlineUsers = new();
        private readonly object _lock = new();

        public bool UserConnected(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_onlineUsers.TryGetValue(userId, out var connections))
                {
                    connections = new HashSet<string>();
                    _onlineUsers[userId] = connections;
                    connections.Add(connectionId);
                    return true;
                }

                connections.Add(connectionId);
                return false;
            }
        }

        public bool UserDisconnected(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_onlineUsers.TryGetValue(userId, out var connections))
                {
                    return false;
                }

                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _onlineUsers.Remove(userId);
                    return true;
                }

                return false;
            }
        }

        public bool IsOnline(string userId)
        {
            lock (_lock)
            {
                return _onlineUsers.ContainsKey(userId);
            }
        }

        public IReadOnlyCollection<string> GetOnlineUsers()
        {
            lock (_lock)
            {
                return _onlineUsers.Keys.ToArray();
            }
        }
    }
}
