using GusMelfordBot.Api.KafkaEventHandlers.Events;
using SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class TelegramMessageReceivedHandler : IEventHandler<TelegramMessageReceivedEvent>
{
    private readonly ILogger<TelegramMessageReceivedHandler> _logger;

    public TelegramMessageReceivedHandler(ILogger<TelegramMessageReceivedHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(TelegramMessageReceivedEvent @event)
    {
        _logger.LogInformation("Message received: {@UpdateText}", @event.Message);
        return Task.CompletedTask;
    }
}