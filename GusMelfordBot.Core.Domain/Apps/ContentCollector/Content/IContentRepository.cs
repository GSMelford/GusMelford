namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;

public interface IContentRepository
{
    IEnumerable<ContentInfo> GetContentList(Filter filter);
    Task SetViewedVideo(Guid contentId);
    Task<DAL.Applications.ContentCollector.Content?> GetContent(Guid contentId);
    Task<long?> GetChatId(Guid chatId);
    Task Cache();
}