﻿using Confluent.Kafka;
using GusMelfordBot.Api.Handlers;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Events;
using GusMelfordBot.Infrastructure;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.SimpleKafka.Interfaces;

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

    public static void SubscribeOnEvents(this WebApplication app, AppSettings appSettings)
    {
        IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
        kafkaConsumerFactory.Subscribe<TelegramMessageReceivedEvent, TelegramMessageReceivedHandler>(BuildConsumerConfig(appSettings));
        kafkaConsumerFactory.Subscribe<ContentProcessedEvent, ContentProcessedHandler>(BuildConsumerConfig(appSettings));
    }

    private static ConsumerConfig BuildConsumerConfig(AppSettings appSettings)
    {
        return new ConsumerConfig { BootstrapServers = appSettings.KafkaSettings!.BootstrapServers };
    }
}