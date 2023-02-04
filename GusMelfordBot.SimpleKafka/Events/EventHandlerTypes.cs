namespace GusMelfordBot.SimpleKafka.Events;

public class EventHandlerTypes
{
    public Type Event { get; }
    public Type Handler { get; }

    public EventHandlerTypes(Type @event, Type handler)
    {
        Event = @event;
        Handler = handler;
    }
}