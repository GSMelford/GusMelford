using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class VideoInformationHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        JToken? token = await GetVideoInformation(processedTikTokContent.OriginalLink);
        if (token is null)
        {
            return processedTikTokContent;
        }

        int.TryParse(token["statusCode"]?.ToString(), out int code);
        processedTikTokContent.VideoStatusCode = code;
        processedTikTokContent.DownloadLink = GetDownloadLink(token);
        return await base.Handle(processedTikTokContent);
    }
    
    private async Task<JToken?> GetVideoInformation(string refererLink)
    {
        string requestUrl = refererLink.BuildVideoInformationUrl();
        RestRequest restRequest = new RestRequest(requestUrl) { Timeout = 60000 };
        restRequest.AddHeader("User-Agent", Constants.UserAgent);

        try {
            return JToken.Parse((await new RestClient().ExecuteAsync(restRequest)).Content!);
        }
        catch {
            return null;
        }
    }
    
    private static string? GetDownloadLink(JToken videoInformation)
    {
        return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["downloadAddr"]?.ToString();
    }
}