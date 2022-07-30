namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentCollectorInfo
{
    public int ContentCount { get; }
    public int Duration { get; }

    public ContentCollectorInfo(int contentCount, int duration)
    {
        ContentCount = contentCount;
        Duration = duration;
    }
}