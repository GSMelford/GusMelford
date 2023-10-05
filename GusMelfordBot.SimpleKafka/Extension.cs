using Confluent.Kafka;
using GusMelfordBot.SimpleKafka.Interfaces;
using GusMelfordBot.SimpleKafka.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.SimpleKafka;

public static class Extension
{
    public static void AddKafkaProducer<TKey>(this IServiceCollection serviceCollection, ProducerConfig config)
    {
        serviceCollection.AddSingleton<IKafkaProducer<TKey>>(provider =>
        {
            ILogger<IKafkaProducer<TKey>>? logger = provider.GetService<ILogger<IKafkaProducer<TKey>>>();
            return new KafkaProducer<TKey>(logger, config);
        });
    }
    
    public static void AddKafkaConsumersFactory(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
    }
}