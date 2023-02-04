namespace GusMelfordBot.SimpleKafka.Events;

public abstract class BaseEvent
{
    public Guid SessionId { get; set; }
}