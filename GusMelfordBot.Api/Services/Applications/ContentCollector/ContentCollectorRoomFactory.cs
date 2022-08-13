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
    
    public List<string> GetUsers(string roomCode)
    {
        return _rooms[roomCode].Users;
    }
    
    public void AddUser(string roomCode, string userId)
    {
        _rooms[roomCode].Users.Add(userId);
    }
    
    public ContentCollectorRoom? FindRoomByRoomCode(string roomCode)
    {
        _rooms.TryGetValue(roomCode, out ContentCollectorRoom? contentCollectorRoom);
        return contentCollectorRoom;
    }

    public ContentCollectorRoom? FindRoomByUserId(string userId)
    {
        var room = _rooms.FirstOrDefault(x=>x.Value.Users.Contains(userId));
        if (room.Value is not null)
        {
           return room.Value;
        }

        return null;
    }
}