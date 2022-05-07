using GusMelfordBot.DAL;
using GusMelfordBot.DAL.Applications.ContentCollector;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;

public interface ITikTokRepository
{
    Task<Chat?> GetChatAsync(long chatId);
    Task<User?> GetUserAsync(long telegramUserId);
    Task<Content?> GetContentAsync(string refererLink);
    Task<int> GetCountAsync();
    Task AddAndSaveContentAsync(Content content);
    TEntity? FirstOrDefault<TEntity>(Func<TEntity, bool> predicate) where TEntity : DatabaseEntity;
    Task UpdateAndSaveContentAsync(Content? content);
    Task<Content?> GetContentAsync(Guid contentId);
}