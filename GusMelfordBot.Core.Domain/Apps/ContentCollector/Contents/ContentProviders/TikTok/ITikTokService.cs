using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;

public interface ITikTokService
{
    Task ProcessMessageAsync(Message message);
    Task<bool> PullAndUpdateContentAsync(Guid contentId, long chatId);
}