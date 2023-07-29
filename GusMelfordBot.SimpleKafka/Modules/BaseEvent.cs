namespace Kyoto.Kafka.Modules;

public abstract class BaseEvent
{
    public Guid SessionId { get; set; }
}