using GusMelfordBot.Extensions.Services.Ftp;

namespace ContentCollector.Settings;

public class AppSettings
{
    public KafkaSettings KafkaSettings { get; set; } = new ();
    public FtpSettings FtpSettings { get; set; } = null!;
}