using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Features.WatchTogether;

public interface IWatchTogetherRoomFactory
{
    string Create(List<ContentDomain> contents);
    WatchTogetherRoom? GetRoomByRoomCode(string roomCode);
    WatchTogetherRoom? GetRoomByUserId(Guid userId);
    void DestroyRoomIfEmpty(string roomCode);
}