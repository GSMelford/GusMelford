namespace GusMelfordBot.Api.Dto.ContentCollector;

public class ContentDto
{
    public Guid Id { get; set; }
    public string ContentPath { get; set; }
    public long Number { get; set; }
    public List<UserDto> Users { get; set; }
    public string? Provider { get; set; }
    public string? OriginalLink { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
}