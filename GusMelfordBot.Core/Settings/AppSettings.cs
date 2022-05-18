using GusMelfordBot.Database.Settings;

namespace GusMelfordBot.Core.Settings;

public class AppSettings
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public TelegramBotSettings? TelegramBotSettings { get; set; }
    public DatabaseSettings? DatabaseSettings { get; set; }
    public FtpServerSettings? FtpServerSettings { get; set; }
}