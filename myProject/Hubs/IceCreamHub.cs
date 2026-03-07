using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace myProject.Hubs;

[Authorize]
public class IceCreamHub : Hub
{
    // keep track of which connections belong to which user identifier
    // we use the built-in UserIdentifier (by default the Name claim)
    private static readonly Dictionary<string, HashSet<string>> _userConnections =
        new Dictionary<string, HashSet<string>>();

    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            lock (_userConnections)
            {
                if (!_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId] = new HashSet<string>();
                }
                _userConnections[userId].Add(Context.ConnectionId);
            }
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            lock (_userConnections)
            {
                if (_userConnections.TryGetValue(userId, out var set))
                {
                    set.Remove(Context.ConnectionId);
                    if (set.Count == 0)
                    {
                        _userConnections.Remove(userId);
                    }
                }
            }
        }
        return base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Called by a client when it adds/updates/deletes something.
    /// the hub will then notify the other connections of the same user only.
    /// </summary>
    public async Task NotifyOthers(string action, string iceCreamName)
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
            return;

        HashSet<string>? connections;
        lock (_userConnections)
        {
            _userConnections.TryGetValue(userId, out connections);
        }

        if (connections != null)
        {
            // exclude the caller's own connection id
            var targets = connections.Where(id => id != Context.ConnectionId).ToList();
            if (targets.Count > 0)
            {
                await Clients.Clients(targets)
                    .SendAsync("ReceiveActivity", userId, action, iceCreamName);
            }
        }
    }

    // legacy broadcast (optional)
    public async Task BroadcastActivity(string userName, string action, string IceCreamName)
    {
        await Clients.All.SendAsync("ReceiveActivity", userName, action, IceCreamName);
    }
}
