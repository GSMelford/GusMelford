using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class ContentProcessedHandler : IEventHandler<ContentProcessedEvent>
{
    private readonly IContentCollectorRepository _contentCollectorRepository;

    public ContentProcessedHandler(IContentCollectorRepository contentCollectorRepository)
    {
        _contentCollectorRepository = contentCollectorRepository;
    }

    public async Task Handle(ContentProcessedEvent @event)
    {
        await _contentCollectorRepository.Update(@event.ToContentProcessed());
    }
}