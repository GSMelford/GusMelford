namespace GusMelfordBot.Core.Services.Data
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Interfaces;
    using DAL.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly IDatabaseManager _databaseManager;
        
        public DataService(IDatabaseManager databaseManager, ILogger<DataService> logger)
        {
            _databaseManager = databaseManager;
            _logger = logger;
        }
        
        public async Task<VideoData> GetUnwatchTikTokVideo()
        {
            List<Video> videos = await _databaseManager.Context
                .Set<Video>()
                .Where(x => !x.IsViewed)
                .ToListAsync();
            
            _logger.LogInformation(
                "New videos were retrieved from the database in the amount of {Count}", videos.Count);
            
            return new VideoData(videos);
        }
        
        public async Task<VideoData> GetTikTokVideo(string takeDateSince, string takeDateUntil)
        {
            List<Video> videos = await _databaseManager.Context
                .Set<Video>()
                .Where(x => 
                    x.CreatedOn <= DateTime.Parse(takeDateSince)
                    && x.CreatedOn < DateTime.Parse(takeDateSince))
                .ToListAsync();
            
            _logger.LogInformation(
                "Videos retrieved from the database from May {TakeDateSince} to April {TakeDateUntil}. " +
                "In the amount of {Count}", takeDateSince, takeDateUntil, videos.Count);
            
            return new VideoData(videos);
        }
    }
}