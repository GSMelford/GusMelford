namespace ContentCollector.Domain.ContentProviders;

public class ProcessedTikTokContent
{
    public Guid ContentId { get; init; }
    public string? Provider { get; init; }
    public string? AccompanyingCommentary { get; init; }
    public string OriginalLink { get; set; } = "";
    public string? Path { get; set; }
    public bool? IsValid { get; set; }
    public int? VideoStatusCode { get; set; }
    public string? DownloadLink { get; set; }
    public byte[] Bytes { get; set; } = Array.Empty<byte>();
    public bool IsSaved { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
}