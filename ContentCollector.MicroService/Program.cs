using Confluent.Kafka;
using ContentCollector.Domain.ContentProviders.TikTok;
using ContentCollector.Domain.System;
using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.KafkaEventHandlers.Handlers;
using ContentCollector.Services.ContentProviders.TikTok;
using ContentCollector.Services.System;
using ContentCollector.Settings;
using SimpleKafka;
using SimpleKafka.Interfaces;

var builder = WebApplication.CreateBuilder(args);
AppSettings appSettings = new AppSettings();
builder.Configuration.Bind(nameof(AppSettings), appSettings);

builder.Services.AddSingleton(appSettings);
builder.Services.AddTransient<ITikTokService, TikTokService>();
builder.Services.AddTransient<IFtpServerService, FtpServerService>();
builder.Services.AddKafkaProducer<string>(new ProducerConfig { BootstrapServers = appSettings.KafkaSettings.BootstrapServers });
builder.Services.AddKafkaConsumersFactory();

var app = builder.Build();

IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
kafkaConsumerFactory.Subscribe<ContentCollectorMessageEvent, ContentCollectorMessageHandler>(new ConsumerConfig
{
    BootstrapServers = "localhost"
});

app.Run();