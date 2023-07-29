using Confluent.Kafka;
using Kyoto.Kafka.Interfaces;
using Newtonsoft.Json;

namespace Kyoto.Kafka.Services;

public class KafkaProducerService<TKey> : IKafkaProducer<TKey>
{
    private readonly IProducer<TKey?, string> _producer;
    
    public KafkaProducerService(ProducerConfig config)
    {
        _producer = new ProducerBuilder<TKey?, string>(config).Build();
    }

    public async Task<DeliveryResult<TKey?, string>> ProduceAsync<TEvent>(TEvent eventData, string? topicPrefix = null, TKey? key = default)
    {
        string topic = typeof(TEvent).Name;
        if (!string.IsNullOrEmpty(topicPrefix)) {
            topic = $"{topicPrefix}.{topic}";
        }
        
        DeliveryResult<TKey?, string> deliveryResult = await _producer.ProduceAsync(topic, new Message<TKey?, string>
        {
            Key = key,
            Value = JsonConvert.SerializeObject(eventData)
        });
        
        _producer.Flush();
        return deliveryResult;
    }

    public void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }
}