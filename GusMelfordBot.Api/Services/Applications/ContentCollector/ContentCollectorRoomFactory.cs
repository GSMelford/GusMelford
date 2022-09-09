using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public class ContentCollectorRoomFactory : IContentCollectorRoomFactory
{
    private readonly Dictionary<string, ContentCollectorRoom> _rooms = new ();

    public string Create(List<ContentDomain> contents)
    {
        string roomCode = new Random().Next(0, 9999).ToString("D");
        _rooms.Add(roomCode, new ContentCollectorRoom(roomCode, contents));
        return roomCode;
    }
    
    public ContentCollectorRoom? GetRoomByRoomCode(string roomCode)
    {
        _rooms.TryGetValue(roomCode, out ContentCollectorRoom? contentCollectorRoom);
        return contentCollectorRoom;
    }

    public ContentCollectorRoom? GetRoomByUserId(Guid userId)
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