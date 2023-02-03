using GusMelfordBot.Api.Services.Features.Abyss;
using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Features.WatchTogether;

public class WatchTogetherRoomFactory : IWatchTogetherRoomFactory
{
    private readonly Dictionary<string, WatchTogetherRoom> _rooms = new ();

    public string Create(List<ContentDomain> contents)
    {
        string roomCode = new Random().Next(0, 9999).ToString("D");
        _rooms.Add(roomCode, new WatchTogetherRoom(roomCode, contents));
        return roomCode;
    }
    
    public WatchTogetherRoom? GetRoomByRoomCode(string roomCode)
    {
        _rooms.TryGetValue(roomCode, out WatchTogetherRoom? contentCollectorRoom);
        return contentCollectorRoom;
    }

    public WatchTogetherRoom? GetRoomByUserId(Guid userId)
    {
        var room = _rooms.FirstOrDefault(x => x.Value.IsUserExist(userId));
        return room.Value ?? null;
    }

    public void DestroyRoomIfEmpty(string roomCode)
    {
        if (GetRoomByRoomCode(roomCode)?.IsRoomEmpty() == true)
        {
            _rooms.Remove(roomCode);
        }
    }
}