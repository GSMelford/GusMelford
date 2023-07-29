using GusMelfordBot.Domain.Telegram.Models;
using Kyoto.Kafka.Modules;

namespace GusMelfordBot.Events;

public class TelegramMessageReceivedEvent : BaseEvent
{
    public MessageDomain? Message { get; set; }
}