namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;

public class ContentInfo
{
    public Guid Id { get; set; }
    public string SenderName { get; set; }
    public string SentLink { get; set; }
    public string AccompanyingCommentary { get; set; }
    public string ContentProvider { get; set; }
}