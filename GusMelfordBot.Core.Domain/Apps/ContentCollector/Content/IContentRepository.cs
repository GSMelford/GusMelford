namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;

public interface IContentRepository
{
    IEnumerable<ContentInfo> GetContentList(Filter filter);
    Task SetViewedVideo(Guid contentId);
}