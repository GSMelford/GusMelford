namespace ContentProcessor.Worker.Domain.ContentProviders.TikTok;

public class ProcessTikTokContent
{
    public Guid Id { get; init; }
    public string Provider { get; init; } = null!;
    public string OriginalLink { get; set; } = null!;
    public string? UserComment { get; init; }
    public int Attempt { get; set; }
    public string? DownloadLink { get; set; }
    public byte[] Bytes { get; set; } = Array.Empty<byte>();
    public bool IsSaved { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string GroupId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public ProcessTikTokContent MarkAsProcessFailed()
    {
        Attempt++;
        return this;
    }
}