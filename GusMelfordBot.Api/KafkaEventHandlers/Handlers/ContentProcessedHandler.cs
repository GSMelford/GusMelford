using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application.ContentCollector;
using SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class ContentProcessedHandler : IEventHandler<ContentProcessedEvent>
{
    private readonly IContentCollectorRepository _contentCollectorRepository;

    public ContentProcessedHandler(IContentCollectorRepository contentCollectorRepository)
    {
        _contentCollectorRepository = contentCollectorRepository;
    }

    public Task Handle(ContentProcessedEvent @event)
    {
        _contentCollectorRepository.Update(@event.ToContentProcessed());
        return Task.CompletedTask;
    }
}