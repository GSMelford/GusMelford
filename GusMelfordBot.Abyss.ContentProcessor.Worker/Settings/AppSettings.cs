namespace ContentProcessor.Worker.Settings;

public class AppSettings
{
    public int Attempt { get; set; } = 15;
    public int MinuteTimeBetweenAttempts { get; set; } = 2;
    public int NumberOfAttempt { get; set; } = 10;
    public string BootstrapServers { get; set; } = "localhost";
}