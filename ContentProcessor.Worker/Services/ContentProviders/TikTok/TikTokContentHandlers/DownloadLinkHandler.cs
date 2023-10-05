using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Domain;
using RestSharp;

namespace ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class DownloadLinkHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent processTikTokContent)
    {
        byte[]? bytes = await DownloadVideo(processTikTokContent.DownloadLink, processTikTokContent.OriginalLink);
        if (bytes is null || bytes.Length < 1000) {
            return processTikTokContent.MarkAsProcessFailed();
        }

        processTikTokContent.Bytes = bytes;
        return await base.HandleAsync(processTikTokContent);
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