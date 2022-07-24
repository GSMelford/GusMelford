namespace ContentCollector.MircoService.Domain.ContentProviders.TikTok;

public class ProcessedContent
{
    public Guid ContentId { get; set; }
    public string Provider { get; set; }
    public string OriginalLink { get; set; }
    public string Path { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public bool? IsValid { get; set; }
    public int VideoStatusCode { get; set; }
    public string? DownloadLink { get; set; }
    public byte[] Bytes { get; set; }
    public bool IsSaved { get; set; }
}