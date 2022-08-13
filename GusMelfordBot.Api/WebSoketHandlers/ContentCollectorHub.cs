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

    public async Task JoinToRoom(object roomCode)
    {
        _contentCollectorRoomFactory.AddUser(roomCode.ToString()!, Context.ConnectionId);
        var room = _contentCollectorRoomFactory.FindRoomByRoomCode(roomCode.ToString()!);
            
        await Clients.All.SendAsync("UserJoined", new
        {
            Users = room?.Users,
            RoomCode = room?.RoomCode
        });
    }
    
    public async Task NextContent(string roomCode)
    {
        _contentCollectorRoomFactory.FindRoomByRoomCode(roomCode)?.Next();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task PrevContent(string roomCode)
    {
        _contentCollectorRoomFactory.FindRoomByRoomCode(roomCode)?.Prev();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task ChangePause(string roomCode)
    {
        bool isPaused = _contentCollectorRoomFactory.FindRoomByRoomCode(roomCode)?.ChangePause() ?? false;
        await Clients.All.SendAsync("PauseChanged", isPaused);
    }
    
    public async Task ChangeRotate(string roomCode)
    {
        int rotate = _contentCollectorRoomFactory.FindRoomByRoomCode(roomCode)?.ChangeRotate() ?? 90;
        await Clients.All.SendAsync("RotateChanged", rotate);
    }

    public override async Task<Task> OnDisconnectedAsync(Exception? exception)
    {
        var room = _contentCollectorRoomFactory.FindRoomByUserId(Context.ConnectionId);
        room?.Users.Remove(Context.ConnectionId);
        
        await Clients.All.SendAsync("UserLeft", new
        {
            Users = room?.Users,
            RoomCode = room?.RoomCode
        });
        
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task StartWatch(string roomCode)
    {
        await Clients.All.SendAsync("StartWatch", roomCode);
    }
    
    public async Task ChangeVideoTime(double currentTime)
    {
        await Clients.Others.SendAsync("ChangeVideoTime", currentTime);
    }
}