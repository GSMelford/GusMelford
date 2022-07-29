using Confluent.Kafka;
using GusMelfordBot.Api.Services.Applications;
using GusMelfordBot.Api.Services.Telegram;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Application;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Extensions.Services.Ftp;
using GusMelfordBot.Infrastructure;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Repositories.Application;
using GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;
using GusMelfordBot.Infrastructure.Repositories.Auth;
using SimpleKafka;
using TBot.Client;

namespace GusMelfordBot.Api;

public static class DependencyInjectionConfigure
{
    public static void ConfigureServices(this IServiceCollection serviceCollection, AppSettings appSettings)
    {
        serviceCollection.AddHttpClient();
        serviceCollection.AddSignalR();
        serviceCollection.AddTBotClient(appSettings.TelegramBotSettings.Token);
        serviceCollection.AddTransient<IDatabaseContext, DatabaseContext>(_ =>
            new DatabaseContext(appSettings.DatabaseSettings));
        serviceCollection.AddTransient<IUpdateService, UpdateService>();
        serviceCollection.AddTransient<IAuthRepository, AuthRepository>();
        serviceCollection.AddTransient<IApplicationRepository, ApplicationRepository>();
        serviceCollection.AddTransient<IContentCollectorRepository, ContentCollectorRepository>();
        serviceCollection.AddTransient<IContentCollectorService, ContentCollectorService>();
        serviceCollection.AddTransient<IFtpServerService, FtpServerService>(
            provider => new FtpServerService(appSettings.FtpSettings, 
                provider.GetRequiredService<ILogger<IFtpServerService>>()));
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