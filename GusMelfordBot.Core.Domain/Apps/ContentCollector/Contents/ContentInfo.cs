namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;

public class ContentInfo
{
    public Guid? Id { get; set; }
    public string? SenderName { get; set; }
    public string? RefererLink { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public string? ContentProvider { get; set; }
    public long? ChatId { get; set; }
}