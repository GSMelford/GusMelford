using Confluent.Kafka;

namespace GusMelfordBot.SimpleKafka.Interfaces;

public interface IKafkaConsumerFactory : IDisposable
{
    void Subscribe<TEvent, THandler>(
        ConsumerConfig? config = null,
        string? topic = null,
        string? groupId = null,
        bool? enableAutoCommit = true)
        where THandler : IEventHandler<TEvent>;
}