namespace GusMelfordBot.Core.Applications.MemesChatApp;

using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GusMelfordBot.Core.Interfaces;
using Services.Requests;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
    
public class VideoDownloader
{
    private readonly IRequestService _requestService;
    private readonly ILogger _logger;
        
    public VideoDownloader(
        IRequestService requestService, 
        ILogger logger)
    {
        _requestService = requestService;
        _logger = logger;
    }
        
    public async Task<byte[]> DownloadTikTokVideo(TikTokVideoContent video)
    {
        try
        {
            JToken videoInformation = await GetVideoInformation(video);
            string originalLink = GetOriginalLink(videoInformation);
            string description = GetDescription(videoInformation);

            video.Description = description;
            
            if (string.IsNullOrEmpty(originalLink))
            {
                return null;
            }
                
            HttpRequestMessage requestMessage = new Request(originalLink)
                .AddHeaders(new Dictionary<string, string>
                {
                    {"User-Agent", Constants.UserAgent},
                    {"Referer", video.RefererLink},
                }).Build();

            HttpResponseMessage httpResponseMessage = await _requestService.ExecuteAsync(requestMessage);
            return await httpResponseMessage.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            _logger.LogError("Download Video error {RefererLink}", video.RefererLink);
            return null;
        }
    }
        
    private static string GetOriginalLink(JToken videoInformation)
    {
        return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["downloadAddr"]?.ToString();
    }
        
    private static string GetDescription(JToken videoInformation)
    {
        return videoInformation["seoProps"]?["metaParams"]?["description"]?.ToString();
    }
        
    private async Task<JToken> GetVideoInformation(TikTokVideoContent video)
    {
        HttpRequestMessage requestMessage =
            new Request(BuildVideoInformationUrl(video))
                .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                .Build();

        return await (await _requestService.ExecuteAsync(requestMessage)).GetJTokenAsync();
    }
        
    private string BuildVideoInformationUrl(TikTokVideoContent video)
    {
        return $"https://www.tiktok.com/node/share/video/{GetVideoUser(video.RefererLink)}" +
               $"/{GetVideoId(video.RefererLink)}";
    }
        
    private static string GetVideoUser(string referer)
    {
        return Regex
            .Match(referer, "com/(.*?)/video")
            .Groups[1]
            .Value;
    }

    private static string GetVideoId(string referer)
    {
        return referer
            .Replace(Constants.TikTok, "")
            .Replace("/video/", " ")
            .Split(" ")[1];
    }
}