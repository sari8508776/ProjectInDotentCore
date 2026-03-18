using Microsoft.AspNetCore.SignalR;
using myProject.Hubs;

namespace myProject.Services;

public interface IActivityRepository
{
    Task BroadcastAsync(string username, string action, string itemName);
}

public class ActivityRepository : IActivityRepository
{
    private readonly IHubContext<ActivityHub> hub;

    public ActivityRepository(IHubContext<ActivityHub> hub)
    {
        this.hub = hub;
    }

    public Task BroadcastAsync(string username, string action, string itemName)
    {
        var connectionIds = ActivityHub.GetConnectionIds(username);
        if (connectionIds.Count == 0)
            return Task.CompletedTask;

        return hub.Clients.Clients(connectionIds)
            .SendAsync("ReceiveActivity", username, action, itemName);
    }
}