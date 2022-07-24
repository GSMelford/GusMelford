using Confluent.Kafka;
using GusMelfordBot.Api.Services.Telegram;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Application;
using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Infrastructure;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Repositories.Application;
using GusMelfordBot.Infrastructure.Repositories.Auth;
using SimpleKafka;

namespace GusMelfordBot.Api;

public static class DependencyInjectionConfigure
{
    public static void ConfigureServices(this IServiceCollection serviceCollection, AppSettings appSettings)
    {
        serviceCollection.AddTransient<IDatabaseContext, DatabaseContext>(_ => new DatabaseContext(appSettings.DatabaseSettings));
        serviceCollection.AddTransient<IUpdateService, UpdateService>();
        serviceCollection.AddTransient<IAuthRepository, AuthRepository>();
        serviceCollection.AddTransient<IApplicationRepository, ApplicationRepository>();
        serviceCollection.AddSingleton(appSettings);
        serviceCollection.AddHealthChecks();
        serviceCollection.AddControllers();
        serviceCollection.AddKafkaProducer<string>(new ProducerConfig
        {
            BootstrapServers = appSettings.KafkaSettings.BootstrapServers
        });
        serviceCollection.AddKafkaConsumersFactory();
    }
}