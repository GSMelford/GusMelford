using GusMelfordBot.Domain.Telegram.Models;
using SimpleKafka.Models;

namespace GusMelfordBot.Api.KafkaEventHandlers.Events;

public class TelegramMessageReceivedEvent : BaseEvent
{
    public MessageDomain? Message { get; set; }
}