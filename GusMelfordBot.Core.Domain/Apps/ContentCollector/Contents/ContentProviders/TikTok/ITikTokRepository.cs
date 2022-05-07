using GusMelfordBot.DAL;
using GusMelfordBot.DAL.Applications.ContentCollector;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;

public interface ITikTokRepository
{
    Task<Chat?> GetChatAsync(long chatId);
    Task<User?> GetUserAsync(long telegramUserId);
    Task<DAL.Applications.ContentCollector.Content?> GetContentAsync(string refererLink);
    Task<int> GetCountAsync();
    Task AddAndSaveContentAsync(DAL.Applications.ContentCollector.Content content);
    TEntity? FirstOrDefault<TEntity>(Func<TEntity, bool> predicate) where TEntity : DatabaseEntity;
    Task UpdateAndSaveContentAsync(Content? content);
}