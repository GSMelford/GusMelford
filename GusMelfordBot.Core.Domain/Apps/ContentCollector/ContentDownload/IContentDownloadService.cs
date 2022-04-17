namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;

public interface IContentDownloadService
{
    Task<MemoryStream?> GetFileStreamContent(Guid contentId);
}