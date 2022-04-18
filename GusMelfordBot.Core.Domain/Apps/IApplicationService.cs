using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps;

public interface IApplicationService
{
    Task ProcessMessage(Message message);
    void ProcessCallbackQuery(CallbackQuery callbackQuery);
}