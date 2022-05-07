using GusMelfordBot.Database.Settings;

namespace GusMelfordBot.Core.Settings;

public class CommonSettings
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public TelegramBotSettings? TelegramBotSettings { get; set; }
    public DatabaseSettings? DatabaseSettings { get; set; }
    public GrayLogSettings? GrayLogSettings { get; set; }
    public FtpServerSettings? FtpServerSettings { get; set; }
}