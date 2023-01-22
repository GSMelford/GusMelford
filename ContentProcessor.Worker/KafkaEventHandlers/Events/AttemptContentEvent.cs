using GusMelfordBot.SimpleKafka.Models;

namespace ContentProcessor.Worker.KafkaEventHandlers.Events;

public class AttemptContentEvent : BaseEvent
{
    public string GroupId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Attempt { get; set; }
}