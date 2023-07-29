using System;
using System.Threading.Tasks;
using ContentProcessor.Worker.Abstractions;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using RestSharp;

namespace ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class RefererLinkHandler : AbstractHandler<ProcessTikTokContent>
{
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent processTikTokContent)
    {
        if (processTikTokContent.OriginalLink.Contains("https://www.tiktok.com/@"))
        {
            processTikTokContent.OriginalLink = BuildOriginalLink(new Uri(processTikTokContent.OriginalLink));
            return await base.HandleAsync(processTikTokContent);
        }

        string? refererLink = await GetRefererLinkAsync(processTikTokContent.OriginalLink);
        if (string.IsNullOrEmpty(refererLink)) {
            return processTikTokContent.MarkAsProcessFailed();
        }

        processTikTokContent.OriginalLink = refererLink;
        return await base.HandleAsync(processTikTokContent);
    }
    
    private static async Task<string?> GetRefererLinkAsync(string sentLink)
    {
        Uri? uri = (await new RestClient().ExecuteAsync(
            new RestRequest(sentLink) { Timeout = 60000 })).ResponseUri;
        
        if (uri is null) {
            return null;
        }
        
        return BuildOriginalLink(uri);
    }

    private static string BuildOriginalLink(Uri uri)
    {
        return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
    }
}