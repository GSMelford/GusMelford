using GusMelfordBot.Extensions.Services.Ftp;

namespace ContentCollector.Settings;

public class AppSettings
{
    public KafkaSettings KafkaSettings { get; set; } = null!;
    public FtpSettings FtpSettings { get; set; } = null!;
}