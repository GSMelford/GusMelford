using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public interface IContentCollectorRoomFactory
{
    string Create(List<ContentDomain> contents);
    ContentCollectorRoom? GetRoomByRoomCode(string roomCode);
    ContentCollectorRoom? GetRoomByUserId(Guid userId);
    void DestroyRoomIfEmpty(string roomCode);
}