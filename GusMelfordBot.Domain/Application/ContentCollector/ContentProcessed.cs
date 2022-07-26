namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentProcessed
{
    public Guid ContentId { get; set; }
    public string? Provider { get; set; }
    public string? OriginalLink { get; set; }
    public string? Path { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public bool? IsValid { get; set; }
    public bool IsSaved { get; set; }
}