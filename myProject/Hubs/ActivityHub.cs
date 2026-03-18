using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;

namespace myProject.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    private static readonly ConcurrentDictionary<string, List<string>> _userConnections = new();

    public override Task OnConnectedAsync()
    {
        var username = Context.User?.FindFirst("username")?.Value;
        if (username != null)
        {
            _userConnections.AddOrUpdate(
                username,
                new List<string> { Context.ConnectionId },
                (key, list) => { list.Add(Context.ConnectionId); return list; }
            );
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.FindFirst("username")?.Value;
        if (username != null && _userConnections.TryGetValue(username, out var list))
        {
            list.Remove(Context.ConnectionId);
            if (list.Count == 0)
                _userConnections.TryRemove(username, out _);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public static IReadOnlyList<string> GetConnectionIds(string username)
    {
        return _userConnections.TryGetValue(username, out var list)
            ? list.ToList()
            : new List<string>();
    }
}
