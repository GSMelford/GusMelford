using GusMelfordBot.SimpleKafka.Models;

namespace ContentProcessor.Worker.KafkaEventHandlers.Events;

public class ContentProcessedEvent : BaseEvent
{
    public string? Provider { get; set; }
    public string? OriginalLink { get; set; }
    public string? UserComment { get; set; }
    public bool IsSaved { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
}