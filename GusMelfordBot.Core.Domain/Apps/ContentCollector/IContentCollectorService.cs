using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector;

public interface IContentCollectorService
{
    void ProcessMessage(Message message);
}