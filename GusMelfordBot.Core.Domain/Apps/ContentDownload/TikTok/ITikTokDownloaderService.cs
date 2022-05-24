using GusMelfordBot.DAL.Applications.ContentCollector;
using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;

public interface ITikTokDownloaderService
{
    Task<bool> TryGetAndSaveRefererLink(Content content);
    Task<JToken> GetVideoInformation(Content content);
    Task<byte[]?> TryDownloadTikTokVideo(string originalLink, string refererLink);
}