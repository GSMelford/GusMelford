using Confluent.Kafka;
using Kyoto.Kafka.Interfaces;
using Kyoto.Kafka.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kyoto.Kafka;

public static class Extension
{
    public static void AddKafkaProducer<TKey>(this IServiceCollection serviceCollection, ProducerConfig config)
    {
        serviceCollection.AddSingleton<IKafkaProducer<TKey>>(_ => new KafkaProducerService<TKey>(config));
    }
    
    public static void AddKafkaConsumersFactory(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IKafkaTopicFactory, KafkaTopicFactory>();
        serviceCollection.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
    }
}