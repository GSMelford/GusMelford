using SimpleKafka.Models;

namespace GusMelfordBot.Api.KafkaEventHandlers.Events;

public class ContentCollectorMessageEvent : BaseEvent
{
    public string MessageText { get; set; }
}