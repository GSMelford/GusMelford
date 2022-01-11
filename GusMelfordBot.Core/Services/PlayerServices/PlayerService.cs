using System.Net.Http;
using GusMelfordBot.Core.Services.Requests;
using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Services.PlayerServices
{
    using Settings;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using DAL.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class PlayerService : IPlayerService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IVideoDownloadService _videoDownloadService;
        private readonly IRequestService _requestService;
        
        private List<Video> _videos;
        private int _cursor = -1;

        public VideoFile CurrentVideoFile { get; private set; }
        public Video CurrentVideo { get; private set; }

        public PlayerService(
            IDatabaseManager databaseManager,
            IVideoDownloadService videoDownloadService,
            IRequestService requestService)
        {
            _databaseManager = databaseManager;
            _videoDownloadService = videoDownloadService;
            _requestService = requestService;
        }

        public async Task Start()
        {
            _cursor = -1;
            _videos = await _databaseManager.Context
                .Set<Video>()
                .Include(video => video.User)
                .Where(x => !x.IsViewed)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();
        }
        
        public async Task<VideoInfo> SetNextVideo()
        {
            Stream stream;
            do
            {
                if (_cursor + 1 >= _videos.Count)
                {
                    _cursor = -1;
                }
                
                if (CurrentVideo is not null)
                {
                    CurrentVideo.IsViewed = true;
                }
                
                CurrentVideo = _videos[++_cursor];
                stream = await GetVideoStream(CurrentVideo);
            } while (stream == null);
            
            _databaseManager.Context.SaveChanges();
            return GetVideoInfoForPlayer();
        }
        
        public async Task<VideoInfo> SetPreviousVideo()
        {
            Stream stream;
            do
            {
                if (_cursor - 1 < 0)
                {
                    _cursor = _videos.Count;
                }

                CurrentVideo = _videos[--_cursor];
                stream = await GetVideoStream(CurrentVideo);
            } while (stream == null);
            
            _databaseManager.Context.SaveChanges();
            return GetVideoInfoForPlayer();
        }

        private VideoInfo GetVideoInfoForPlayer()
        {
            return new VideoInfo
            {
                Id = CurrentVideo?.Id.ToString(),
                DateTime = CurrentVideo?.CreatedOn ?? DateTime.Now,
                RefererLink = CurrentVideo?.RefererLink,
                VideoSenderName = CurrentVideo?.User.FirstName,
                Signature = CurrentVideo?.Signature
            };
        }
        
        private async Task<Stream> GetVideoStream(Video video)
        {
            CurrentVideoFile = await _videoDownloadService.DownloadVideo(video);

            if (!CurrentVideoFile.IsDownloaded)
            {
                video.IsValid = false;
                return null;
            }

            video.IsValid = true;
            return CurrentVideoFile.Stream;
        }

        public void AddNewVideos(JToken videos)
        {
            if (videos["itemList"] is not JArray itemList)
            {
                return;
            }
            
            foreach (JToken item in itemList)
            {
                string link = $"https://www.tiktok.com/@{item["author"]?["nickname"]}/video/{item["video"]?["id"]}";
                Video video = CreateVideo(link);
                _videos.Add(video);
            }
        }
        
        public DAL.TikTok.Video CreateVideo(string link, long userId = 443763853)
        {
            string videoLink = link.Split(new []{' ', '\n'}).FirstOrDefault(l =>
                l.Contains(Constants.TikTokVMDomain) 
                || l.Contains(Constants.TikTokMDomain)
                || l.Contains(Constants.TikTokWWWDomain))?.Trim();

            if (string.IsNullOrEmpty(videoLink))
            {
                return null;
            }
            
            string signature = link.Replace(videoLink, "");
            
            HttpRequestMessage requestMessage =
                new Request(videoLink)
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();

            HttpResponseMessage httpResponseMessage = _requestService.ExecuteAsync(requestMessage).Result;
            Uri uri = httpResponseMessage.RequestMessage?.RequestUri;
            
            requestMessage =
                new Request(uri?.ToString())
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();
            
            httpResponseMessage = _requestService.ExecuteAsync(requestMessage).Result;
            uri = httpResponseMessage.RequestMessage?.RequestUri;
            
            string referer = string.Empty;
            if (uri is not null)
            {
                referer = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
            }

            var user = _databaseManager.Context.Set<DAL.User>()
                .FirstOrDefault(u => u.TelegramUserId == userId);
            
            return new DAL.TikTok.Video {
                User = user,
                SentLink = videoLink,
                RefererLink = referer,
                Signature = signature
            };
        }
    }
}