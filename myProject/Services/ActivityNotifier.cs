using Microsoft.AspNetCore.SignalR;
using myProject.Hubs;

namespace myProject.Services;

public interface IActivityRepository
{
    System.Threading.Tasks.Task BroadcastAsync(string username, string action, string itemName);
}

public class ActivityRepository : IActivityRepository
{
    private readonly IHubContext<ActivityHub> _hub;

    public ActivityRepository(IHubContext<ActivityHub> hub)
    {
        _hub = hub;
    }

    public System.Threading.Tasks.Task BroadcastAsync(string username, string action, string itemName)
    {
        // send the same event name KS uses: ReceiveActivity
        return _hub.Clients.All.SendAsync("ReceiveActivity", username, action, itemName);
    }
}
