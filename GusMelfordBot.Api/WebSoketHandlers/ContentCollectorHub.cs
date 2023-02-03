using GusMelfordBot.Api.Services.Features.Abyss;
using GusMelfordBot.Api.Services.Features.WatchTogether;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace GusMelfordBot.Api.WebSoketHandlers;

public class ContentCollectorHub : Hub
{
    private readonly IWatchTogetherRoomFactory _watchTogetherRoomFactory;
    
    public ContentCollectorHub(
        IWatchTogetherRoomFactory watchTogetherRoomFactory)
    {
        _watchTogetherRoomFactory = watchTogetherRoomFactory;
    }

    public async Task JoinToRoom(object roomCode)
    {
        WatchTogetherRoom? contentCollectorRoom = _watchTogetherRoomFactory.GetRoomByRoomCode(roomCode.ToString()!);
        if (contentCollectorRoom is null) {
            return;
        }
        
        ContentCollectorUser contentCollectorUser = null;
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
        _watchTogetherRoomFactory.GetRoomByRoomCode(roomCode)?.Next();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task PrevContent(string roomCode)
    {
        _watchTogetherRoomFactory.GetRoomByRoomCode(roomCode)?.Prev();
        await Clients.All.SendAsync("VideoChanged");
    }
    
    public async Task ChangePause(string roomCode)
    {
        bool isPaused = _watchTogetherRoomFactory.GetRoomByRoomCode(roomCode)?.ChangePause() ?? false;
        await Clients.All.SendAsync("PauseChanged", isPaused);
    }
    
    public async Task ChangeRotate(string roomCode)
    {
        int rotate = _watchTogetherRoomFactory.GetRoomByRoomCode(roomCode)?.ChangeRotate() ?? 90;
        await Clients.All.SendAsync("RotateChanged", rotate);
    }

    public override async Task<Task> OnDisconnectedAsync(Exception? exception)
    {
        Guid userId = Context.GetHttpContext()!.GetUserId();
        WatchTogetherRoom? contentCollectorRoom = _watchTogetherRoomFactory.GetRoomByUserId(userId);
        if (contentCollectorRoom is null) {
            return base.OnDisconnectedAsync(exception);
        }
        
        contentCollectorRoom.RemoveUser(userId);
        _watchTogetherRoomFactory.DestroyRoomIfEmpty(contentCollectorRoom.RoomCode);
        
        await Clients.All.SendAsync("UserLeft", new
        {
            Users = contentCollectorRoom.GetUsers(),
            contentCollectorRoom.RoomCode
        });
        
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task StartWatch(string roomCode)
    {
        WatchTogetherRoom? contentCollectorRoom = _watchTogetherRoomFactory.GetRoomByRoomCode(roomCode);
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