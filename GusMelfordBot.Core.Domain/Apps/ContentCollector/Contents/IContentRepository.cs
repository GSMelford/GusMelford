using GusMelfordBot.DAL.Applications.ContentCollector;

namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;

public interface IContentRepository
{
    IEnumerable<ContentInfoDomain> GetContentList(Filter filter);
    Task SetViewedVideo(Guid contentId);
    Task<Content?> GetContent(Guid contentId);
    Task<long?> GetChatId(Guid chatId);
    IEnumerable<Content> GetUnfinishedContents();
}