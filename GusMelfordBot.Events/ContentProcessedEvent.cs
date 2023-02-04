using GusMelfordBot.SimpleKafka.Models;

namespace GusMelfordBot.Events;

public class ContentProcessedEvent : BaseEvent
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = null!;
    public string OriginalLink { get; set; } = null!;
    public string? UserComment { get; set; }
    public bool IsSaved { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
}