using GusMelfordBot.Domain.Telegram.Models;
using GusMelfordBot.SimpleKafka.Models;

namespace GusMelfordBot.Events;

public class TelegramMessageReceivedEvent : BaseEvent
{
    public MessageDomain? Message { get; set; }
}