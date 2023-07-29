using Confluent.Kafka;
using GusMelfordBot.Api.Handlers;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Events;
using GusMelfordBot.Infrastructure;
using GusMelfordBot.Infrastructure.Interfaces;
using Kyoto.Kafka.Interfaces;
using TBot.Client;
using TBot.Client.Api.Telegram.SendMessage;

namespace GusMelfordBot.Api;

public static class WebApplicationExtensions
{
    public static AppSettings BindAppSettings(this ConfigurationManager configurationManager)
    {
        AppSettings appSettings = new AppSettings();
        configurationManager.Bind(nameof(AppSettings), appSettings);
        return appSettings;
    }

    public static void SetEnvironmentSettings(this WebApplication app, WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())  
        {  
            app.UseDeveloperExceptionPage();  
        }
        else
        {
            app.UseHsts();
        }
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app, DatabaseSettings databaseSettings)
    {
        IDatabaseContext databaseContext = app.Services.GetRequiredService<IDatabaseContext>();
        await databaseContext.InitializeDatabaseAsync(databaseSettings);
    }

    public static async Task SubscribeOnEvents(this WebApplication app, AppSettings appSettings)
    {
        IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
        await kafkaConsumerFactory.SubscribeAsync<TelegramMessageReceivedEvent, TelegramMessageReceivedHandler>(BuildConsumerConfig(appSettings));
        await kafkaConsumerFactory.SubscribeAsync<ContentProcessedEvent, ContentProcessedHandler>(BuildConsumerConfig(appSettings));
        await kafkaConsumerFactory.SubscribeAsync<AttemptContentEvent, AttemptContentHandler>(BuildConsumerConfig(appSettings));
    }

    private static ConsumerConfig BuildConsumerConfig(AppSettings appSettings)
    {
        return new ConsumerConfig { BootstrapServers = appSettings.KafkaSettings!.BootstrapServers };
    }

    public static async Task NotifyAboutRestartAsync(this WebApplication app)
    {
        //TODO Dev tool
        ITBot tBot = app.Services.GetRequiredService<ITBot>();
        await tBot.SendMessageAsync(new SendMessageParameters
        {
            Text = "🤖 The bot has been restarted.",
            ChatId = -1001529315725
        });
    }
}