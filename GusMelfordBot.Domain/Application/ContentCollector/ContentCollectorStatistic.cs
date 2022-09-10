namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentCollectorStatistic
{
    public Dictionary<string, int> UserNewContents { get; set; } = null!;
    public int NotViewedVideoCount { get; set; }
    public int Duration { get; set; }
}