namespace GusMelfordBot.SimpleKafka.Models;

public abstract class BaseEvent
{
    public Guid SessionId { get; set; }
}