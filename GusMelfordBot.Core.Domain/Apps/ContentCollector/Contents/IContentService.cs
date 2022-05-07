namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;

public interface IContentService
{
    List<ContentInfo> BuildContentInfoList(Filter filter);
    Task SetViewedVideo(Guid contentId);
    Task Cache();
    int Refresh(long chatId);
}