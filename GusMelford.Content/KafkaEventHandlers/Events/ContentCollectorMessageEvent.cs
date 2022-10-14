using GusMelfordBot.SimpleKafka.Models;

namespace ContentCollector.KafkaEventHandlers.Events;

public class ContentCollectorMessageEvent : BaseEvent
{
    public string MessageText { get; set; }
}