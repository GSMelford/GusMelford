using GusMelfordBot.Api.Services.Applications.ContentCollector;
using Microsoft.AspNetCore.SignalR;

namespace GusMelfordBot.Api.WebSoketHandlers;

public class ContentCollectorHub : Hub
{
    private readonly IContentCollectorRoomFactory _contentCollectorRoomFactory;
    
    public ContentCollectorHub(IContentCollectorRoomFactory contentCollectorRoomFactory)
    {
        _contentCollectorRoomFactory = contentCollectorRoomFactory;
    }

    public async Task JoinToRoom(string roomCode)
    {
        _contentCollectorRoomFactory.AddUser(roomCode, Context.ConnectionId);
        await Clients.All.SendAsync("UserJoined", Context.ConnectionId);
    }
    
    public async Task NextContent(string roomCode)
    {
        _contentCollectorRoomFactory.GetContentCollectorRoom(roomCode).Next();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task PrevContent(string roomCode)
    {
        _contentCollectorRoomFactory.GetContentCollectorRoom(roomCode).Prev();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task ChangePause(string roomCode)
    {
        bool isPaused = _contentCollectorRoomFactory.GetContentCollectorRoom(roomCode).ChangePause();
        await Clients.All.SendAsync("PauseChanged", isPaused);
    }
    
    public async Task ChangeRoute(string roomCode)
    {
        int route = _contentCollectorRoomFactory.GetContentCollectorRoom(roomCode).ChangeRoute();
        await Clients.All.SendAsync("RouteChanged", route);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _contentCollectorRoomFactory.RemoveUser(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}