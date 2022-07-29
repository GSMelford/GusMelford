namespace GusMelfordBot.SimpleKafka.Interfaces;

public interface IEventHandler<in TEvent>
{
    Task Handle(TEvent @event);
}