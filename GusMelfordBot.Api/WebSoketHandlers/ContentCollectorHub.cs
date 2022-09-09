using GusMelfordBot.Api.Services.Applications.ContentCollector;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace GusMelfordBot.Api.WebSoketHandlers;

public class ContentCollectorHub : Hub
{
    private readonly IContentCollectorRoomFactory _contentCollectorRoomFactory;
    private readonly IContentCollectorRepository _contentCollectorRepository;
    
    public ContentCollectorHub(
        IContentCollectorRoomFactory contentCollectorRoomFactory, 
        IContentCollectorRepository contentCollectorRepository)
    {
        _contentCollectorRoomFactory = contentCollectorRoomFactory;
        _contentCollectorRepository = contentCollectorRepository;
    }

    public async Task JoinToRoom(object roomCode)
    {
        ContentCollectorRoom? contentCollectorRoom = _contentCollectorRoomFactory.GetRoomByRoomCode(roomCode.ToString()!);
        if (contentCollectorRoom is null) {
            return;
        }
        
        ContentCollectorUser contentCollectorUser = await _contentCollectorRepository.GetUserAsync(Context.GetHttpContext()!.GetUserId());
        contentCollectorUser.ConnectionId = Context.ConnectionId;
        contentCollectorRoom.AddUser(contentCollectorUser);
        
        await Clients.All.SendAsync("UserJoined", new
        {
            Users = contentCollectorRoom.GetUsers(),
            contentCollectorRoom.RoomCode
        });
    }
    
    public async Task NextContent(string roomCode)
    {
        _contentCollectorRoomFactory.GetRoomByRoomCode(roomCode)?.Next();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task PrevContent(string roomCode)
    {
        _contentCollectorRoomFactory.GetRoomByRoomCode(roomCode)?.Prev();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task ChangePause(string roomCode)
    {
        bool isPaused = _contentCollectorRoomFactory.GetRoomByRoomCode(roomCode)?.ChangePause() ?? false;
        await Clients.All.SendAsync("PauseChanged", isPaused);
    }
    
    public async Task ChangeRotate(string roomCode)
    {
        int rotate = _contentCollectorRoomFactory.GetRoomByRoomCode(roomCode)?.ChangeRotate() ?? 90;
        await Clients.All.SendAsync("RotateChanged", rotate);
    }

    public override async Task<Task> OnDisconnectedAsync(Exception? exception)
    {
        Guid userId = Context.GetHttpContext()!.GetUserId();
        ContentCollectorRoom? contentCollectorRoom = _contentCollectorRoomFactory.GetRoomByUserId(userId);
        if (contentCollectorRoom is null) {
            return base.OnDisconnectedAsync(exception);
        }
        
        contentCollectorRoom.RemoveUser(userId);
        _contentCollectorRoomFactory.DestroyRoomIfEmpty(contentCollectorRoom.RoomCode);
        
        await Clients.All.SendAsync("UserLeft", new
        {
            Users = contentCollectorRoom.GetUsers(),
            contentCollectorRoom.RoomCode
        });
        
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task StartWatch(string roomCode)
    {
        ContentCollectorRoom? contentCollectorRoom = _contentCollectorRoomFactory.GetRoomByRoomCode(roomCode);
        if (contentCollectorRoom is null) {
            return;
        }
        
        contentCollectorRoom.SetReady(Context.GetHttpContext()!.GetUserId());
        if (contentCollectorRoom.GetUsers().All(x=>x.IsReady)) 
        {
            await Clients.All.SendAsync("StartWatch", roomCode);
        }
        else
        {
            await Clients.All.SendAsync("UserJoined", new
            {
                Users = contentCollectorRoom.GetUsers(),
                contentCollectorRoom.RoomCode
            });
        }
    }
    
    public async Task ChangeVideoTime(double currentTime)
    {
        await Clients.Others.SendAsync("ChangeVideoTime", currentTime);
    }
}