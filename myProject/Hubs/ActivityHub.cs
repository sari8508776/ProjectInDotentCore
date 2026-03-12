using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace myProject.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    // Broadcasts a simple activity (username, action, itemName) to all clients
    public async Task BroadcastActivity(string username, string action, string itemName)
    {
        await Clients.All.SendAsync("ReceiveActivity", username, action, itemName);
    }
}
