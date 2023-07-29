namespace Kyoto.Kafka.Interfaces;

public interface IKafkaHandler<in TEvent>
{
    Task HandleAsync(TEvent @event);
}