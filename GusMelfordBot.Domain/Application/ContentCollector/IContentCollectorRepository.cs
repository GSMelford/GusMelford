namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IContentCollectorRepository
{
    Task<bool> SaveNew(Guid contentId, long? chatId, long? telegramUserId, string messageText, long? messageId);
    Task<Guid> Update(ContentProcessed contentProcessed);
    IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter);
    Task<string?> GetContentPath(Guid contentId);
    Task<ContentCollectorInfo> GetContentCollectorInfo(ContentFilter contentFilter);
    Task<long?> GetChatId(Guid contentId);
    Task<string> GetVideoCaption(Guid contentId);
    Task MarkContentAsViewed(Guid contentId);
}