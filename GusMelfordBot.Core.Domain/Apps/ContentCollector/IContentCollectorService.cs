using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector;

public interface IContentCollectorService
{
    void ProcessMessage(Message message);
    Task<Message?> SendInformationPanelAsync(Guid contentId);
    void DeleteInformationPanelAsync(Guid chatId, int messageId);
    void ProcessCallbackQuery(CallbackQuery callbackQuery);
}