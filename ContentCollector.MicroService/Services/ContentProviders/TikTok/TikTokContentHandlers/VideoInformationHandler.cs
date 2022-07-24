using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class VideoInformationHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedContent?> Handle(ProcessedContent processedContent)
    {
        JToken? token = await GetVideoInformation(processedContent.OriginalLink);
        if (token is null)
        {
            return processedContent;
        }

        int.TryParse(token["statusCode"]?.ToString(), out int code);
        processedContent.VideoStatusCode = code;
        processedContent.DownloadLink = GetDownloadLink(token);
        return await base.Handle(processedContent);
    }
    
    private async Task<JToken?> GetVideoInformation(string refererLink)
    {
        string requestUrl = refererLink.BuildVideoInformationUrl();
        RestRequest restRequest = new RestRequest(requestUrl) { Timeout = 60000 };
        restRequest.AddHeader("User-Agent", Constants.USER_AGENT);

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