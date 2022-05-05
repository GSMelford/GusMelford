using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;

public interface ITikTokService
{
    Task ProcessMessageAsync(Message message);
}