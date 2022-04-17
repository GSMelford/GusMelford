using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;

public interface ITikTokService
{
    Task ProcessMessage(Message message);
}