namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentDomain
{
    public Guid Id { get; }
    public string ContentPath { get; }
    public long Number { get; }
    public List<ContentUserDomain> Users { get; private set; }
    public string? Provider { get; }
    public string? OriginalLink { get; }
    public string? AccompanyingCommentary { get; }
    public int? Height { get; }
    public int? Width { get; }
    public int? Duration { get; }

    public ContentDomain(
        Guid id,
        string contentPath,
        long number,
        List<ContentUserDomain> users,
        string? provider, 
        string? originalLink,
        string? accompanyingCommentary,
        int? height,
        int? width,
        int? duration)
    {
        Id = id;
        ContentPath = contentPath;
        Number = number;
        Users = users;
        Provider = provider;
        OriginalLink = originalLink;
        AccompanyingCommentary = accompanyingCommentary;
        Height = height;
        Width = width;
        Duration = duration;
    }

    public void SetUsers(List<ContentUserDomain> userDomains)
    {
        Users = userDomains;
    }
}