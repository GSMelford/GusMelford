using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Extensions;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.Core.Services.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;

public class TikTokDownloadManager
{
    private readonly ILogger<ContentDownloadService> _logger;
    private readonly IRequestService _requestService;
        
    public TikTokDownloadManager(
        IRequestService requestService, 
        ILogger<ContentDownloadService> logger)
    {
        _logger = logger;
        _requestService = requestService;
    }
        
    public async Task<byte[]?> DownloadTikTokVideo
        (DAL.Applications.ContentCollector.Content? content)
    {
        if (content is null)
        {
            return null;
        }
        
        try
        {
            JToken videoInformation = await GetVideoInformation(content);
            string? originalLink = GetOriginalLink(videoInformation);

            content.Description = GetDescription(videoInformation);
            
            if (string.IsNullOrEmpty(originalLink))
            {
                return null;
            }
                
            Request request = new Request
            {
                HttpMethod = HttpMethod.Get,
                RequestUri = originalLink,
                Headers = new Dictionary<string, string>
                {
                    {"User-Agent", Constants.UserAgent},
                    {"Referer", content.RefererLink},
                }
            };
            
            HttpResponseMessage httpResponseMessage = await _requestService.ExecuteAsync(request.ToHttpRequestMessage());
            return await httpResponseMessage.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            _logger?.LogError("Video Download Error {RefererLink}", content.RefererLink);
            return null;
        }
    }
        
    private static string? GetOriginalLink(JToken videoInformation)
    {
        return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["downloadAddr"]?.ToString();
    }
        
    private static string? GetDescription(JToken videoInformation)
    {
        return videoInformation["seoProps"]?["metaParams"]?["description"]?.ToString();
    }
        
    private async Task<JToken> GetVideoInformation(DAL.Applications.ContentCollector.Content content)
    {
        Request request = new Request
        {
            HttpMethod = HttpMethod.Get,
            RequestUri = BuildVideoInformationUrl(content),
            Headers = new Dictionary<string, string>
            {
                {"User-Agent", Constants.UserAgent}
            }
        };
        
        return await (await _requestService.ExecuteAsync(request.ToHttpRequestMessage())).GetJTokenAsyncOrEmpty();
    }
        
    private string BuildVideoInformationUrl(DAL.Applications.ContentCollector.Content content)
    {
        return $"https://www.tiktok.com/node/share/video/{TikTokServiceHelper.GetUserName(content.RefererLink)}" +
               $"/{TikTokServiceHelper.GetVideoId(content.RefererLink)}";
    }
}