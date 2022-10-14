using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Domain.Telegram.Models;
using GusMelfordBot.SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.Services.Telegram;

public class UpdateService : IUpdateService
{
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly IAuthRepository _authRepository;

    public UpdateService(IKafkaProducer<string> kafkaProducer, IAuthRepository authRepository)
    {
        _kafkaProducer = kafkaProducer;
        _authRepository = authRepository;
    }
    
    public async Task ProcessUpdate(UpdateDomain update)
    {
        if (update.Message is not null)
        {
            Guid userId = await RegisterIfNotExist(update.Message.From);
            await _kafkaProducer.ProduceAsync(new TelegramMessageReceivedEvent
            {
                Id = userId,
                Message = update.Message
            });
        }
    }

    private async Task<Guid> RegisterIfNotExist(TelegramObjectUserDomain? userDomain)
    {
        if (userDomain is null) {
            throw new Exception();
        }

        return await _authRepository.RegisterUserFromTelegramIfNotExist(
            new RegisterData(
                userDomain.FirstName!,
                userDomain.LastName,
                string.Empty,
                string.Empty),
            userDomain.Username,
            userDomain.Id ?? throw new Exception());
    }
}