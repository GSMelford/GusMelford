using Confluent.Kafka;
using GusMelfordBot.SimpleKafka.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GusMelfordBot.SimpleKafka.Services;

public class KafkaProducer<TKey> : IKafkaProducer<TKey>
{
    private readonly ILogger<IKafkaProducer<TKey>>? _logger;
    private readonly IProducer<TKey?, string> _producer;
    
    public KafkaProducer(ILogger<IKafkaProducer<TKey>>? logger, ProducerConfig config)
    {
        _logger = logger;
        _producer = new ProducerBuilder<TKey?, string>(config).Build();
    }

    public async Task<DeliveryResult<TKey?, string>> ProduceAsync<TEvent>(TEvent eventData, string? topic = null, TKey? key = default)
    {
        if (string.IsNullOrEmpty(topic)) {
            topic = typeof(TEvent).Name;
        }
        
        _logger?.LogInformation("Publication in the {Topic} topic.", topic);
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