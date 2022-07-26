using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using RestSharp;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class DownloadLinkHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        byte[]? bytes = await DownloadVideo(processedTikTokContent.DownloadLink, processedTikTokContent.OriginalLink);
        if (bytes is null) {
            return processedTikTokContent;
        }

        processedTikTokContent.Bytes = bytes;
        return await base.Handle(processedTikTokContent);
    }
    
    private async Task<byte[]?> DownloadVideo(string? downloadLink, string refererLink)
    {
        if (string.IsNullOrEmpty(downloadLink)) {
            return null;
        }
        
        try
        {
            RestRequest request = new RestRequest(downloadLink);
            request.AddHeader("User-Agent", Constants.UserAgent);
            request.AddHeader("Referer", refererLink);
            return (await new RestClient().ExecuteAsync(request)).RawBytes;
        }
        catch
        {
            return null;
        }
    }
}