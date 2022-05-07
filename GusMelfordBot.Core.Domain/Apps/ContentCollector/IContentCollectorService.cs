using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector;

public interface IContentCollectorService
{
    Task ProcessMessage(Message message);
    void ProcessCallbackQuery(CallbackQuery callbackQuery);
    Task<int> Refresh(long chatId);
}