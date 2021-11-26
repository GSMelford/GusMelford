namespace GusMelfordBot.Core.Services.PlayerServices
{
    using System.Text.RegularExpressions;
    using DAL.TikTok;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Interfaces;
    using Requests;
    using Microsoft.Extensions.Logging;
    
    public class VideoDownloadService : IVideoDownloadService
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<VideoDownloadService> _logger;
        
        public VideoDownloadService(
            IRequestService requestService, 
            ILogger<VideoDownloadService> logger)
        {
            _requestService = requestService;
            _logger = logger;
        }
        
        public async Task<VideoFile> DownloadVideo(Video video)
        {
            string originalLink = GetOriginalLink(await GetVideoInformation(video));

            if (string.IsNullOrEmpty(originalLink))
            {
                return new VideoFile();
            }
            
            try
            {
                HttpRequestMessage requestMessage = new Request(originalLink)
                    .AddHeaders(new Dictionary<string, string>
                    {
                        {"User-Agent", Constants.UserAgent},
                        {"Referer", video.RefererLink},
                    }).Build();

                HttpResponseMessage httpResponseMessage = await _requestService.ExecuteAsync(requestMessage);
                byte[] videoArray = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                Stream videoStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                return new VideoFile
                {
                    IsDownloaded = true,
                    Stream = videoStream,
                    VideoArray = videoArray
                };
            }
            catch
            {
                _logger.LogError("Download Video error {RefererLink}", video.RefererLink);
                return new VideoFile();
            }
        }
        
        private string GetOriginalLink(JToken videoInformation)
        {
            return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["downloadAddr"]?.ToString();
        }
        
        private async Task<JToken> GetVideoInformation(Video video)
        {
            HttpRequestMessage requestMessage =
                new Request(BuildVideoInformationUrl(video))
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();

            return await (await _requestService.ExecuteAsync(requestMessage)).GetJTokenAsync();
        }
        
        private string BuildVideoInformationUrl(Video video)
        {
            return $"https://www.tiktok.com/node/share/video/{GetVideoUser(video.RefererLink)}" +
                   $"/{GetVideoId(video.RefererLink)}";
        }
        
        private string GetVideoUser(string referer)
        {
            return Regex
                .Match(referer, "com/(.*?)/video")
                .Groups[1]
                .Value;
        }

        private string GetVideoId(string referer)
        {
            return referer
                .Replace(Constants.TikTokDomain, "")
                .Replace("/video/", " ")
                .Split(" ")[1];
        }
    }
}