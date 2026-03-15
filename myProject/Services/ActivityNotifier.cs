using Microsoft.AspNetCore.SignalR;
using myProject.Hubs;

namespace myProject.Services;

public interface IActivityRepository

{
    System.Threading.Tasks.Task BroadcastAsync(string username, string action, string itemName);
}

public class ActivityRepository : IActivityRepository
{
    private readonly IHubContext<ActivityHub> hub;

    public ActivityRepository(IHubContext<ActivityHub> hub)
    {
        this.hub = hub;
    }

    public System.Threading.Tasks.Task BroadcastAsync(string username, string action, string itemName)
    {
        return this.hub.Clients.All.SendAsync("ReceiveActivity", username, action, itemName);
    }
}
