namespace GusMelfordBot.Core.Applications.MemesChatApp.Player
{
    using GusMelfordBot.Core.Interfaces;
    using Entities;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using Microsoft.EntityFrameworkCore;
    
    public class PlayerService : IPlayerService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IRequestService _requestService;
        private readonly ILogger<PlayerService> _logger;
        
        private List<TikTokVideoContent> _videos;
        private int _cursor = -1;

        private TikTokVideoContent CurrentContent { get; set; }
        public byte[] CurrentContentBytes { get; set; }

        public PlayerService(
            ILogger<PlayerService> logger,
            IDatabaseManager databaseManager,
            IRequestService requestService)
        {
            _databaseManager = databaseManager;
            _requestService = requestService;
            _logger = logger;

            Init();
        }

        private void Init()
        {
            _videos = _databaseManager.Context
                .Set<TikTokVideoContent>()
                .Include(video => video.User)
                .Where(x => !x.IsViewed)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync().Result;
        }
        
        public async Task<PlayerInfo> SetNextVideo()
        {
            do
            {
                if (_cursor + 1 >= _videos.Count)
                {
                    _cursor = -1;
                }
                
                if (CurrentContent is not null)
                {
                    CurrentContent.IsViewed = true;
                }
                
                CurrentContent = _videos[++_cursor];
                CurrentContentBytes = await GetVideoBytes(CurrentContent);
            } while (CurrentContentBytes == null);
            
            await _databaseManager.Context.SaveChangesAsync();
            return GetVideoInfoForPlayer();
        }
        
        public async Task<PlayerInfo> SetPreviousVideo()
        {
            do
            {
                if (_cursor - 1 < 0)
                {
                    _cursor = _videos.Count;
                }

                CurrentContent = _videos[--_cursor];
                CurrentContentBytes = await GetVideoBytes(CurrentContent);
            } while (CurrentContentBytes == null);
            
            await _databaseManager.Context.SaveChangesAsync();
            return GetVideoInfoForPlayer();
        }

        private PlayerInfo GetVideoInfoForPlayer()
        {
            return new PlayerInfo
            {
                SenderName = CurrentContent.User.FirstName + " " + CurrentContent.User.LastName,
                AccompanyingCommentary = CurrentContent.AccompanyingCommentary,
                AuthorDescription = CurrentContent.Description,
                SentDateTime = CurrentContent.CreatedOn.ToString("g")
            };
        }
        
        private async Task<byte[]> GetVideoBytes(TikTokVideoContent video)
        {
            VideoDownloader videoDownloader = new VideoDownloader(_requestService, _logger);
            return await videoDownloader.DownloadTikTokVideo(video);
        }
    }
}