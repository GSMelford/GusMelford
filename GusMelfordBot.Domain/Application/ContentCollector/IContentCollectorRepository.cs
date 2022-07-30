namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IContentCollectorRepository
{
    Task Create(Guid contentId, long? chatId, long? telegramUserId, string messageText, long? messageId);
    Task Update(ContentProcessed contentProcessed);
    IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter);
    Task<string?> GetContentPath(Guid contentId);
    Task<ContentCollectorInfo> GetContentCollectorInfo(ContentFilter contentFilter);
}