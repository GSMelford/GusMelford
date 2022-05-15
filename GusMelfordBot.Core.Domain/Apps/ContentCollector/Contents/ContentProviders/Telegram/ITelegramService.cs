using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.Telegram;

public interface ITelegramService
{
    Task ProcessPhoto(Message message);
}