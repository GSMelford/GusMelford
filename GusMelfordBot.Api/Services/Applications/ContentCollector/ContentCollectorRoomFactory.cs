using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public class ContentCollectorRoomFactory : IContentCollectorRoomFactory
{
    private readonly Dictionary<string, ContentCollectorRoom> _rooms = new ();

    public string Create(List<ContentDomain> contents)
    {
        string roomCode = new Random().Next(0, 9999).ToString("D");
        _rooms.Add(roomCode, new ContentCollectorRoom(contents));
        return roomCode;
    }
    
    public void AddUser(string roomCode, string userId)
    {
        _rooms[roomCode].Users.Add(userId);
    }
    
    public void RemoveUser(string userId)
    {
        _rooms.FirstOrDefault(x=>x.Value.Users.Contains(userId)).Value.Users.Remove(userId);
    }

    public ContentCollectorRoom GetContentCollectorRoom(string roomCode)
    {
        return _rooms[roomCode];
    }
}