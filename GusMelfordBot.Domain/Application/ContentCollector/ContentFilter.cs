namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentFilter
{
    public bool IsViewed { get; }

    public ContentFilter(bool isViewed)
    {
        IsViewed = isViewed;
    }
}