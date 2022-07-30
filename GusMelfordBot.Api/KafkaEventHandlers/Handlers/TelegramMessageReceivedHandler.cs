using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.SimpleKafka.Interfaces;
using TBot.Client;
using TBot.Client.Api.Telegram.DeleteMessage;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class TelegramMessageReceivedHandler : IEventHandler<TelegramMessageReceivedEvent>
{
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IContentCollectorRepository _contentCollectorRepository;
    private readonly ITBot _tBot;

    public TelegramMessageReceivedHandler(
        IKafkaProducer<string> kafkaProducer,
        IApplicationRepository applicationRepository,
        IContentCollectorRepository contentCollectorRepository, 
        ITBot tBot)
    {
        _kafkaProducer = kafkaProducer;
        _applicationRepository = applicationRepository;
        _contentCollectorRepository = contentCollectorRepository;
        _tBot = tBot;
    }

    public async Task Handle(TelegramMessageReceivedEvent @event)
    {
        long? chatId = @event.Message?.Chat?.Id;
        if (chatId is not null)
        {
            switch (await _applicationRepository.GetApplicationService(chatId.Value))
            {
                case ApplicationService.ContentCollector:
                    await HandleContentCollector(@event);
                    await _tBot.DeleteMessageAsync(new DeleteMessageParameters
                    {
                        ChatId = chatId,
                        MessageId = @event.Message!.MessageId!.Value
                    });
                    break;
                case ApplicationService.Unknown:
                    break;
            }
        }
    }

    private async Task HandleContentCollector(TelegramMessageReceivedEvent @event)
    {
        string? messageText = @event.Message?.Text;
        if (string.IsNullOrEmpty(messageText)) {
            return;
        }
                   
        Guid contentId = Guid.NewGuid();
        await _contentCollectorRepository.Create(
            contentId,
            @event.Message?.Chat?.Id,
            @event.Message?.From?.Id,
            messageText,
            @event.Message?.Chat?.Id);

        await _kafkaProducer.ProduceAsync(new ContentCollectorMessageEvent
        {
            Id = contentId,
            MessageText = messageText
        });
    }
}