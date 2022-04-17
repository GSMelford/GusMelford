using GusMelfordBot.DAL;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;

public interface ITikTokRepository
{
    Task<Chat?> GetChatAsync(long chatId);
    Task<User?> GetUserAsync(long telegramUserId);
    Task<DAL.Applications.ContentCollector.Content?> GetContentAsync(string refererLink);
    Task<int> GetCountAsync();
    Task SaveContentAsync(DAL.Applications.ContentCollector.Content content);
}