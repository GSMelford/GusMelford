using GusMelfordBot.DAL.Applications.ContentCollector;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;

public interface ITikTokService
{
    Task ProcessMessageAsync(Message message);
    void PullAndUpdateContent(Content content, long chatId);
}