using Microsoft.AspNetCore.SignalR;

namespace Demesne.Server.Hubs;

public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Welcome", new
        {
            message = "Connected to Demesne",
            serverTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
        await base.OnConnectedAsync();
    }
}
