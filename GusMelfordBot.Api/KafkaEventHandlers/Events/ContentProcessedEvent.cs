using SimpleKafka.Models;

namespace GusMelfordBot.Api.KafkaEventHandlers.Events;

public class ContentProcessedEvent : BaseEvent
{
    public string Provider { get; set; }
    public string OriginalLink { get; set; }
    public string Path { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public bool? IsValid { get; set; }
    public bool IsSaved { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
}