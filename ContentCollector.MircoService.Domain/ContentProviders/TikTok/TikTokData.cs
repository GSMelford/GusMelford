namespace ContentCollector.MircoService.Domain.ContentProviders.TikTok;

public class TikTokData
{
    public Guid ContentId { get; }
    public string Link { get; }
    public string? AccompanyingCommentary { get; }

    public TikTokData(Guid contentId, string link, string? accompanyingCommentary)
    {
        ContentId = contentId;
        Link = link;
        AccompanyingCommentary = accompanyingCommentary;
    }
}