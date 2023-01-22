using Confluent.Kafka;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using ContentProcessor.Worker.KafkaEventHandlers.Events;
using ContentProcessor.Worker.KafkaEventHandlers.Handlers;
using ContentProcessor.Worker.Services.ContentProviders.TikTok;
using ContentProcessor.Worker.Settings;
using GusMelfordBot.Extensions.Services.DataLake;
using GusMelfordBot.SimpleKafka;
using GusMelfordBot.SimpleKafka.Interfaces;

var builder = WebApplication.CreateBuilder(args);
AppSettings appSettings = new AppSettings();
builder.Configuration.Bind(nameof(AppSettings), appSettings);

builder.Services.AddControllers();
builder.Services.AddSingleton(appSettings);
builder.Services.AddTransient<ITikTokService, TikTokService>();
builder.Services.AddTransient<IDataLakeService, DataLakeService>();

builder.Services.AddKafkaProducer<string>(new ProducerConfig { BootstrapServers = appSettings.KafkaSettings.BootstrapServers });
builder.Services.AddKafkaConsumersFactory();

var app = builder.Build();

app.MapGet("/", () => "ContentProcessor.Worker 3.0");
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
kafkaConsumerFactory.Subscribe<ContentEvent, ContentHandler>(new ConsumerConfig
{
    BootstrapServers = appSettings.KafkaSettings.BootstrapServers
});

app.Run();