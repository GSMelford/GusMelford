using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using RestSharp;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class DownloadLinkHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedContent?> Handle(ProcessedContent processedContent)
    {
        byte[]? bytes = await DownloadVideo(processedContent.DownloadLink, processedContent.OriginalLink);
        if (bytes is null) {
            return processedContent;
        }

        processedContent.Bytes = bytes;
        return await base.Handle(processedContent);
    }
    
    private async Task<byte[]?> DownloadVideo(string? downloadLink, string refererLink)
    {
        if (string.IsNullOrEmpty(downloadLink)) {
            return null;
        }
        
        try
        {
            RestRequest request = new RestRequest(downloadLink);
            request.AddHeader("User-Agent", Constants.USER_AGENT);
            request.AddHeader("Referer", refererLink);
            return (await new RestClient().ExecuteAsync(request)).RawBytes;
        }
        catch
        {
            return null;
        }
    }
}