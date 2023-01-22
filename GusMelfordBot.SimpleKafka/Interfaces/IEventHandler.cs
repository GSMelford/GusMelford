namespace GusMelfordBot.SimpleKafka.Interfaces;

public interface IEventHandler<in TEvent>
{
    Task HandleAsync(TEvent @event);
}