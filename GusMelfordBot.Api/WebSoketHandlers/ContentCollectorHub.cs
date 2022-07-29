using Microsoft.AspNetCore.SignalR;

namespace GusMelfordBot.Api.WebSoketHandlers;

public class ContentCollectorHub : Hub
{
    public async Task Send(string message)
    {
        await Clients.All.SendAsync("Send", message);
    }
}