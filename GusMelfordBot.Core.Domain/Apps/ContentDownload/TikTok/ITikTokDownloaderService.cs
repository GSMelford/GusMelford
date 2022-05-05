namespace GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;

public interface ITikTokDownloaderService
{
    Task<byte[]?> DownloadTikTokVideo(DAL.Applications.ContentCollector.Content? content);
}