using Confluent.Kafka;
using GusMelfordBot.Api.Services.Telegram;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Infrastructure;
using GusMelfordBot.Infrastructure.Interfaces;
using SimpleKafka;

namespace GusMelfordBot.Api;

public static class DependencyInjectionConfigure
{
    public static void ConfigureServices(this IServiceCollection serviceCollection, AppSettings appSettings)
    {
        serviceCollection.AddTransient<IDatabaseContext, DatabaseContext>();
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