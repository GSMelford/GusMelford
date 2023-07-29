using Kyoto.Kafka.Modules;

namespace Kyoto.Kafka.Interfaces;

public interface IKafkaConsumerService : IDisposable
{
    event EventHandler<ReceivedEventDetails> Received;
    public void Consume(string topic, string groupId, bool enableAutoCommit);
}