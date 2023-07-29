namespace Kyoto.Kafka.Interfaces;

public interface IKafkaTopicFactory
{
    Task CreateTopicIfNotExistAsync(string topic, Dictionary<string, string> config);
}