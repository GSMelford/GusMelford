namespace GusMelfordBot.Api.Settings;

public class AbyssSettings
{
    public int Attempt { get; set; } = 15;
    public int MinuteTimeBetweenAttempts { get; set; } = 2;
    public int NumberOfAttempt { get; set; } = 10;
}