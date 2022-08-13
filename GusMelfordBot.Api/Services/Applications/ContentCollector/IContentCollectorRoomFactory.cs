using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public interface IContentCollectorRoomFactory
{
    string Create(List<ContentDomain> contents);
    void AddUser(string roomCode, string userId);
    List<string> GetUsers(string roomCode);
    ContentCollectorRoom? FindRoomByRoomCode(string roomCode);
    ContentCollectorRoom? FindRoomByUserId(string userId);
}