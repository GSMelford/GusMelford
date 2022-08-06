using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public interface IContentCollectorRoomFactory
{
    string Create(List<ContentDomain> contents);
    void AddUser(string roomCode, string userId);
    void RemoveUser(string userId);
    ContentCollectorRoom GetContentCollectorRoom(string roomCode);
}