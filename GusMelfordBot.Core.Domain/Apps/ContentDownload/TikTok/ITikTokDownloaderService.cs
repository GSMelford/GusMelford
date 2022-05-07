using GusMelfordBot.DAL.Applications.ContentCollector;

namespace GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;

public interface ITikTokDownloaderService
{
    Task<byte[]?> DownloadTikTokVideo(Content? content);
    Task<bool> TryGetAndSaveRefererLink(Content content);
}