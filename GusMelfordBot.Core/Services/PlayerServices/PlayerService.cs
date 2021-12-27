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

        private List<Video> _videos;
        private int _cursor = -1;

        public VideoFile CurrentVideoFile { get; private set; }
        public Video CurrentVideo { get; private set; }

        public PlayerService(
            IDatabaseManager databaseManager,
            IVideoDownloadService videoDownloadService)
        {
            _databaseManager = databaseManager;
            _videoDownloadService = videoDownloadService;
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
    }
}