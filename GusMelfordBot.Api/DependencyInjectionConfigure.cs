using Confluent.Kafka;
using GusMelfordBot.Api.Services.Telegram;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Telegram;
using SimpleKafka;

namespace GusMelfordBot.Api;

public static class DependencyInjectionConfigure
{
    public static void ConfigureServices(this IServiceCollection serviceCollection, AppSettings appSettings)
    {
        serviceCollection.AddSingleton(appSettings);
        serviceCollection.AddHealthChecks();
        serviceCollection.AddControllers();
        serviceCollection.AddTransient<IUpdateService, UpdateService>();
        serviceCollection.AddKafkaProducer<string>(new ProducerConfig
        {
            BootstrapServers = appSettings.KafkaSettings.BootstrapServers
        });
        serviceCollection.AddKafkaConsumersFactory();
    }
}