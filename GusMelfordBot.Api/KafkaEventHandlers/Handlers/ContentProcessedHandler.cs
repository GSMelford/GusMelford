using GusMelfordBot.Api.KafkaEventHandlers.Events;
using SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class ContentProcessedHandler : IEventHandler<ContentProcessedEvent>
{
    public Task Handle(ContentProcessedEvent @event)
    {
        return Task.CompletedTask;
    }
}