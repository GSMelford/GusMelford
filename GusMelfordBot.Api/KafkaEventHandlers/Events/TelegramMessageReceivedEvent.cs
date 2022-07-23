using SimpleKafka.Models;
using TBot.Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Api.KafkaEventHandlers.Events;

public class TelegramMessageReceivedEvent : BaseEvent
{
    public Message? Message { get; set; }
}