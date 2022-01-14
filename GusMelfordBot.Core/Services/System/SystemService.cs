using System.Threading.Tasks;
using GusMelfordBot.Core.Interfaces;
using GusMelfordBot.Core.Services.Data.Entities;
using GusMelfordBot.Core.Settings;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Services.System
{
    public class SystemService : ISystemService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly CommonSettings _commonSettings;

        public SystemService(
            IDatabaseManager databaseManager,
            CommonSettings commonSettings)
        {
            _databaseManager = databaseManager;
            _commonSettings = commonSettings;
        }
        
        public async Task<SystemInfo> GetSystemData()
        {
            return new SystemInfo
            {
                Name = _commonSettings.Name,
                Version = _commonSettings.Version,
                PlayerInformation = new PlayerInformation
                {
                    Count = await _databaseManager.Context.Set<TikTokVideoContent>().CountAsync(x => !x.IsViewed),
                    Name = $"{_commonSettings.Name} Player",
                    Version = _commonSettings.PlayerVersion
                }
            };
        }
    }
}