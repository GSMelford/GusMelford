using Microsoft.AspNetCore.SignalR;

namespace GusMelfordBot.Api.WebSoketHandlers;

public class ContentCollectorHub : Hub
{
    public async Task ChangeVideoTime(double currentTime)
    {
        await Clients.Others.SendAsync("ChangeVideoTime", currentTime);
    }
    
    public async Task SwitchVideo(string direction)
    {
        await Clients.All.SendAsync("SwitchVideo", direction);
    }
    
    public async Task PauseVideo(bool isPaused)
    {
        await Clients.All.SendAsync("PauseVideo", isPaused);
    }
}