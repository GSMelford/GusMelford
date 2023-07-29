using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Domain.Telegram.Models;
using GusMelfordBot.Events;
using Kyoto.Kafka.Interfaces;

namespace GusMelfordBot.Api.Services.Telegram;

public class TelegramService : IUpdateService
{
    private readonly ILogger<TelegramService> _logger;
    private readonly IKafkaProducer<string> _kafkaProducer;

    private readonly IAuthRepository _authRepository;

    public TelegramService(
        ILogger<TelegramService> logger, 
        IKafkaProducer<string> kafkaProducer, 
        IAuthRepository authRepository)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _authRepository = authRepository;
    }
    
    public async Task ProcessUpdateAsync(UpdateDomain update)
    {
        Guid internalUpdateId = Guid.NewGuid();
        
        _logger.LogInformation("Received new update object from telegram. " +
                               "Update id: {UpdateId}. Internal update id: {InternalUpdateId}", 
            update.UpdateId, internalUpdateId);
        
        if (update.Message is not null)
        {
            await ImplicitRegistrationAsync(update.Message.From!);
            await _kafkaProducer.ProduceAsync(new TelegramMessageReceivedEvent
            {
                SessionId = internalUpdateId,
                Message = update.Message
            });
        }
    }

    private async Task ImplicitRegistrationAsync(TelegramObjectUserDomain userDomain)
    {
        if (!await _authRepository.IsTelegramUserExistAsync(userDomain.Id!.Value))
        {
            await _authRepository.SaveTelegramUserAsync(new RegisterData(
                userDomain.FirstName!,
                userDomain.LastName,
                $"{DateTime.UtcNow:ddMMyyyy.hhmmss}@temp.mail.com",
                string.Empty), userDomain.Id.Value, userDomain.Username!);
        }
    }
}