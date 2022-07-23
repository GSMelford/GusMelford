using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Telegram;
using SimpleKafka.Interfaces;
using TBot.Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Api.Services.Telegram;

public class UpdateService : IUpdateService
{
    private readonly IKafkaProducer<string> _kafkaProducer;

    public UpdateService(IKafkaProducer<string> kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }
    
    public async Task ProcessUpdate(Update update)
    {
        if (update.Message is not null)
        {
            await _kafkaProducer.ProduceAsync(new TelegramMessageReceivedEvent
            {
                Id = Guid.NewGuid(),
                Message = update.Message
            });
        }
    }
}