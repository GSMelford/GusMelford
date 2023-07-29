using GusMelfordBot.Api.Services.Features.Abyss;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;
using Kyoto.Kafka.Interfaces;

namespace GusMelfordBot.Api.Handlers;

public class AttemptContentHandler : IKafkaHandler<AttemptContentEvent>
{
    private readonly IAbyssRepository _abyssRepository;

    public AttemptContentHandler(IAbyssRepository abyssRepository)
    {
        _abyssRepository = abyssRepository;
    }

    public async Task HandleAsync(AttemptContentEvent @event)
    { 
        await _abyssRepository.AddAttemptMessageAsync(@event.ToDomain());
    }
}