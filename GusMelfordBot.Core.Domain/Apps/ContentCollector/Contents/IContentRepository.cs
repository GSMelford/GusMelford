using GusMelfordBot.DAL.Applications.ContentCollector;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;

public interface IContentRepository
{
    IEnumerable<ContentInfo> GetContentList(Filter filter);
    Task SetViewedVideo(Guid contentId);
    Task<DAL.Applications.ContentCollector.Content?> GetContent(Guid contentId);
    Task<long?> GetChatId(Guid chatId);
    Task Cache();
    IEnumerable<Content> GetUnfinishedContents();
}