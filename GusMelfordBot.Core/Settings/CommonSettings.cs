using GusMelfordBot.Database.Settings;

namespace GusMelfordBot.Core.Settings
{
    public class CommonSettings
    {
        public TelegramBotSettings TelegramBotSettings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
    }
}