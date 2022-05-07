namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;

public class ContentInfoDomain
{
    public Guid? Id { get; set; }
    public string? SenderName { get; set; }
    public string? RefererLink { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public string? ContentProvider { get; set; }
    public string? Description { get; set; }
    public long? ChatId { get; set; }
}