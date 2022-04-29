namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;

public interface IContentService
{
    List<ContentInfo> BuildContentInfoList(Filter filter);
    Task SetViewedVideo(Guid contentId);
    Task Cache();
}