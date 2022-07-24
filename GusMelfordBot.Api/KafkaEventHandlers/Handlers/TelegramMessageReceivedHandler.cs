using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application;
using SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class TelegramMessageReceivedHandler : IEventHandler<TelegramMessageReceivedEvent>
{
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly IApplicationRepository _applicationRepository;

    public TelegramMessageReceivedHandler(IKafkaProducer<string> kafkaProducer, IApplicationRepository applicationRepository)
    {
        _kafkaProducer = kafkaProducer;
        _applicationRepository = applicationRepository;
    }

    public async Task Handle(TelegramMessageReceivedEvent @event)
    {
        long? chatId = @event.Message?.Chat?.Id;
        if (chatId is not null)
        {
            switch (await _applicationRepository.GetApplicationService(chatId.Value))
            {
                case ApplicationService.ContentCollector:
                    await _kafkaProducer.ProduceAsync(new ContentCollectorMessageEvent
                    {
                        Id = @event.Id,
                        MessageText = @event.Message?.Text ?? throw new Exception()
                    });
                    break;
                case ApplicationService.Unknown:
                    break;
            }
        }
    }
}