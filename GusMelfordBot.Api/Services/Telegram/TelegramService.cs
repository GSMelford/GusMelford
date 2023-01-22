using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Domain.Telegram.Models;
using GusMelfordBot.Events;
using GusMelfordBot.SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.Services.Telegram;

public class TelegramService : IUpdateService
{
    private readonly ILogger<TelegramService> _logger;
    private readonly IKafkaProducer<string> _kafkaProducer;

    public TelegramService(ILogger<TelegramService> logger, IKafkaProducer<string> kafkaProducer)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }
    
    public async Task ProcessUpdateAsync(UpdateDomain update)
    {
        Guid internalUpdateId = Guid.NewGuid();
        
        _logger.LogInformation("Received new update object from telegram. " +
                               "Update id: {UpdateId}. Internal update id: {InternalUpdateId}", 
            update.UpdateId, internalUpdateId);
        
        if (update.Message is not null)
        {
            await _kafkaProducer.ProduceAsync(new TelegramMessageReceivedEvent
            {
                SessionId = internalUpdateId,
                Message = update.Message
            });
        }
    }
}