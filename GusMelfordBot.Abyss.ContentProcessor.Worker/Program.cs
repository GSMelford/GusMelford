using Confluent.Kafka;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using ContentProcessor.Worker.KafkaHandlers;
using ContentProcessor.Worker.Services.ContentProviders.TikTok;
using ContentProcessor.Worker.Settings;
using GusMelfordBot.Events;
using GusMelfordBot.Extensions.Services.DataLake;
using Kyoto.Kafka;
using Kyoto.Kafka.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
AppSettings appSettings = new AppSettings();
builder.Configuration.Bind(nameof(AppSettings), appSettings);

builder.Services.AddControllers();
builder.Services.AddSingleton(appSettings);
builder.Services.AddTransient<ITikTokService, TikTokService>();
builder.Services.AddTransient<IDataLakeService, DataLakeService>();

builder.Services.AddKafkaProducer<string>(new ProducerConfig { BootstrapServers = appSettings.BootstrapServers });
builder.Services.AddKafkaConsumersFactory();

var app = builder.Build();

app.MapGet("/", () => "GusMelfordBot.Abyss.ContentProcessor.Worker 3.0");
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
await kafkaConsumerFactory.SubscribeAsync<ContentEvent, ContentHandler>(new ConsumerConfig
{
    BootstrapServers = appSettings.BootstrapServers
});

app.Run();