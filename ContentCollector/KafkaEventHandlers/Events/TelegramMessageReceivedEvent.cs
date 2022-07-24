using SimpleKafka.Models;

namespace ContentCollector.KafkaEventHandlers.Events;

public class TelegramMessageReceivedEvent : BaseEvent
{
    public string Message { get; set; }
}