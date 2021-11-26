using GusMelfordBot.Database.Settings;

namespace GusMelfordBot.Core.Settings
{
    public class CommonSettings
    {
        public string Version { get; set; }
        public string PlayerVersion { get; set; }
        public TelegramBotSettings TelegramBotSettings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
        public TikTokSettings TikTokSettings { get; set; }
    }
}