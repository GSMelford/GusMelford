using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.KafkaEventHandlers.Handlers;
using SimpleKafka.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
kafkaConsumerFactory.Subscribe<TelegramMessageReceivedEvent, TelegramMessageReceivedHandler>();

app.Run();