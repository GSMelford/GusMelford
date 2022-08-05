using Confluent.Kafka;
using ContentCollector.Domain.ContentProviders.TikTok;
using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.KafkaEventHandlers.Handlers;
using ContentCollector.Services.ContentProviders.TikTok;
using ContentCollector.Settings;
using GusMelfordBot.DataLake;
using GusMelfordBot.Extensions.Services.Ftp;
using GusMelfordBot.SimpleKafka;
using GusMelfordBot.SimpleKafka.Interfaces;

var builder = WebApplication.CreateBuilder(args);
AppSettings appSettings = new AppSettings();
builder.Configuration.Bind(nameof(AppSettings), appSettings);

builder.Services.AddSingleton(appSettings);
builder.Services.AddTransient<ITikTokService, TikTokService>();
builder.Services.AddTransient<IDataLakeService, DataLakeService>();
builder.Services.AddTransient<IFtpServerService, FtpServerService>(
    provider => new FtpServerService(appSettings.FtpSettings, 
        provider.GetRequiredService<ILogger<IFtpServerService>>()));
builder.Services.AddKafkaProducer<string>(new ProducerConfig { BootstrapServers = appSettings.KafkaSettings.BootstrapServers });
builder.Services.AddKafkaConsumersFactory();

var app = builder.Build();

IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
kafkaConsumerFactory.Subscribe<ContentCollectorMessageEvent, ContentCollectorMessageHandler>(new ConsumerConfig
{
    BootstrapServers = appSettings.KafkaSettings.BootstrapServers
});

app.Run();