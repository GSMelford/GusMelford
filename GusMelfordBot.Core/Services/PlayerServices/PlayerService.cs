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
        private readonly CommonSettings _commonSettings;

        private List<Video> _videos;
        private List<Video> _newVideos;
        private Video _currentVideo;
        private VideoFile _videoFile;
        private int _cursor = -1;

        public PlayerService(
            IDatabaseManager databaseManager,
            IVideoDownloadService videoDownloadService, 
            CommonSettings commonSettings)
        {
            _databaseManager = databaseManager;
            _videoDownloadService = videoDownloadService;
            _commonSettings = commonSettings;
        }

        public async Task Start()
        {
            _videos = await _databaseManager.Context
                .Set<Video>()
                .Include(video => video.User)
                .ToListAsync();

            _newVideos = _videos.Where(x => !x.IsViewed).ToList();
        }

        public Video GetCurrentVideo()
        {
            return _currentVideo;
        }

        public VideoInfo GetVideoInfo()
        {
            return new VideoInfo
            {
                PlayerNameVersion = "GusMelfordBot Player v" +  _commonSettings.PlayerVersion,
                Id = _currentVideo?.Id.ToString(),
                DateTime = _currentVideo?.CreatedOn ?? DateTime.Now,
                RefererLink = _currentVideo?.RefererLink,
                VideoSenderName = _currentVideo?.User.FirstName
            };
        }

        public async Task<VideoInfo> SetNextVideo()
        {
            Stream stream;
            do
            {
                if (_currentVideo is not null)
                {
                    _currentVideo.IsViewed = true;
                    _currentVideo = null;
                }
                
                if (_cursor + 1 < _newVideos.Count)
                {
                    _cursor++;
                }

                _currentVideo = _newVideos[_cursor];
                stream = await GetVideoStream(_currentVideo);

                if (_cursor + 1 == _newVideos.Count && stream == null)
                {
                    _cursor = -1;
                }
                
            } while (stream == null);
            
            await _databaseManager.SaveAllAcync();
            return GetVideoInfo();
        }
        
        public async Task<VideoInfo> SetPreviousVideo()
        {
            Stream stream;
            do
            {
                if (_currentVideo is not null)
                {
                    _currentVideo.IsViewed = true;
                    _currentVideo = null;
                }

                if (_cursor == -1)
                {
                    _cursor = 0;
                }
                
                if (_cursor - 1 > -1)
                {
                    _cursor--;
                }

                _currentVideo = _newVideos[_cursor];
                stream = await GetVideoStream(_currentVideo);

                if (_cursor - 1 == -1 && stream == null)
                {
                    _cursor = _newVideos.Count - 1;
                }
                
            } while (stream == null);
            
            await _databaseManager.SaveAllAcync();
            return GetVideoInfo();
        }

        private async Task<Stream> GetVideoStream(Video video)
        {
            _videoFile = await _videoDownloadService.DownloadVideo(video);

            if (!_videoFile.IsDownloaded)
            {
                video.IsValid = false;
                return null;
            }

            video.IsValid = true;
            return _videoFile.Stream;
        }

        public async Task<Stream> GetCurrentVideoFileStream()
        {
            string bufferName = "temp.mp4";

            if (File.Exists(bufferName))
            {
                File.Delete(bufferName);
            }

            await using (FileStream fileStream = new FileStream(bufferName, FileMode.CreateNew))
            {
                await fileStream.WriteAsync(_videoFile.VideoArray, 0, _videoFile.VideoArray.Length);
            }

            return File.OpenRead(bufferName);
        }

        public Stream GetCurrentVideoStream()
        {
            return _videoFile.Stream;
        }
    }
}