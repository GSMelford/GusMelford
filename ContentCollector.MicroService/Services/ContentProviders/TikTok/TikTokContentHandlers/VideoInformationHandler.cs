using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Extensions;
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
        processedTikTokContent.Height = GetHeightContent(token);
        processedTikTokContent.Width = GetWidthContent(token);
        processedTikTokContent.Duration = GetDurationContent(token);
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
    
    private static int? GetHeightContent(JToken videoInformation)
    {
        return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["height"]?.ToString().ToInt();
    }
    
    private static int? GetWidthContent(JToken videoInformation)
    {
        return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["width"]?.ToString().ToInt();
    }
    
    private static int? GetDurationContent(JToken videoInformation)
    {
        return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["duration"]?.ToString().ToInt();
    }
}