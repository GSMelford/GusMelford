using System.Text.RegularExpressions;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.Core.Services.Requests;
using GusMelfordBot.DAL.Applications.ContentCollector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;

public class TikTokDownloaderService : ITikTokDownloaderService
{
    private readonly ILogger<TikTokDownloaderService> _logger;
    private readonly IRequestService _requestService;
        
    public TikTokDownloaderService(
        IRequestService requestService, 
        ILogger<TikTokDownloaderService> logger)
    {
        _logger = logger;
        _requestService = requestService;
    }
        
    public async Task<byte[]?> DownloadTikTokVideo(Content? content)
    {
        if (content is null)
        {
            return null;
        }
        
        try
        {
            JToken videoInformation = await GetVideoInformation(content);
            if (int.TryParse(videoInformation["statusCode"]?.ToString(), out int code))
            {
                if (code is > 10000 and < 11000)
                {
                    content.IsValid = false;
                    _logger.LogInformation("When receiving a link to content, received a code {Code}", code);
                    return null; 
                }
            }
            
            string? originalLink = GetOriginalLink(videoInformation);
            if (string.IsNullOrEmpty(originalLink))
            {
                _logger.LogWarning("The original link is not available at the moment." +
                                   " Request token {Token}", videoInformation);
                return null;
            }
            
            content.Description = GetDescription(videoInformation);
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
            _logger.LogError("Video Download Error {RefererLink}", content.RefererLink);
            return null;
        }
    }
        
    public async Task<bool> TryGetAndSaveRefererLink(Content content)
    {
        if (string.IsNullOrEmpty(content.RefererLink))
        {
            content.RefererLink = await GetRefererLink(content.SentLink);
            if (string.IsNullOrEmpty(content.RefererLink))
            {
                _logger.LogError("Not available referer link from the link " +
                                    "{SentLink} ContentId: {ContentId}", content.SentLink, content.Id);
                return false;
            }
            
            _logger.LogInformation("ContentId: {ContentId} RefererLink: {RefererLink}", content.Id, content.RefererLink);
        }

        return true;
    }
    
    private static string? GetOriginalLink(JToken videoInformation)
    {
        return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["downloadAddr"]?.ToString();
    }
        
    private static string GetDescription(JToken videoInformation)
    {
        string? description = videoInformation["seoProps"]?["metaParams"]?["description"]?.ToString();
        if (!string.IsNullOrEmpty(description))
        {
            return new Regex("\\S* Likes, \\S* Comments. TikTok video from \\D*: \"(\\D*)\"")
                .Match(description).Groups[1].Value;
        }
        
        return string.Empty;
    }
        
    private async Task<JToken> GetVideoInformation(Content content)
    {
        string requestUrl = BuildVideoInformationUrl(content);
        RestClient restClient = new RestClient();
        RestRequest restRequest = new RestRequest(requestUrl) { Timeout = 60000 };
        restRequest.AddHeader("User-Agent", Constants.UserAgent);
        RestResponse restResponse = await restClient.ExecuteAsync(restRequest);

        try
        {
            return JToken.Parse(restResponse.Content!);
        }
        catch
        {
            _logger.LogError("Failed Request {Request}  ContentId: {ContentId}", requestUrl, content.Id);
            return new JObject();
        }
    }
        
    private static async Task<string> GetRefererLink(string? sentLink)
    {
        RestClient restClient = new RestClient();
        RestRequest restRequest = new RestRequest(sentLink) { Timeout = 60000 };
        RestResponse restResponse = await restClient.ExecuteAsync(restRequest);
        
        Uri? uri = restResponse.ResponseUri;
        if (uri is null)
        {
            return string.Empty;
        }
        
        return uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
    }
    
    private string BuildVideoInformationUrl(Content content)
    {
        return $"https://www.tiktok.com/node/share/video/{TikTokServiceHelper.GetUserName(content.RefererLink)}" +
               $"/{TikTokServiceHelper.GetVideoId(content.RefererLink)}";
    }
}