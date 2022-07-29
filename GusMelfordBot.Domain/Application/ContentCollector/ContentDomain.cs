namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentDomain
{
    public long Number { get; }
    public UserDomain User { get; }
    public string? Provider { get; }
    public string? OriginalLink { get; }
    public string? AccompanyingCommentary { get; }
    public int? Height { get; }
    public int? Width { get; }
    public int? Duration { get; }

    public ContentDomain(
        long number,
        UserDomain user,
        string? provider, 
        string? originalLink,
        string? accompanyingCommentary,
        int? height,
        int? width,
        int? duration)
    {
        Number = number;
        User = user;
        Provider = provider;
        OriginalLink = originalLink;
        AccompanyingCommentary = accompanyingCommentary;
        Height = height;
        Width = width;
        Duration = duration;
    }
}