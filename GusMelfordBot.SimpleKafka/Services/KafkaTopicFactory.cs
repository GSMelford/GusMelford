using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Kyoto.Kafka.Interfaces;

namespace Kyoto.Kafka.Services;

public class KafkaTopicFactory : IKafkaTopicFactory
{
    private const int ATTEMPTS = 5;
    
    public async Task CreateTopicIfNotExistAsync(string topic, Dictionary<string, string> config)
    {
        using var adminClient = new AdminClientBuilder(config).Build();
        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(1));

        if (metadata.Topics.All(x => x.Topic != topic))
        {
            for (var i = 0; i < ATTEMPTS; i++)
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new [] { new TopicSpecification
                    {
                        Name = topic,
                        NumPartitions = 10,
                        ReplicationFactor = 1
                    }});
                    return;
                }
                catch
                {
                    //
                }
            }
        }
    }
}