using GusMelfordBot.Api.Services.Features.Abyss;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;
using Kyoto.Kafka.Interfaces;

namespace GusMelfordBot.Api.Handlers;

public class ContentProcessedHandler : IKafkaHandler<ContentProcessedEvent>
{
    private readonly ILogger<IKafkaHandler<ContentProcessedEvent>> _logger;
    private readonly IAbyssService _abyssService;

    public ContentProcessedHandler(ILogger<IKafkaHandler<ContentProcessedEvent>> logger,IAbyssService abyssService)
    {
        _logger = logger;
        _abyssService = abyssService;
    }
    
    public async Task HandleAsync(ContentProcessedEvent @event)
    {
        var processedContent = @event.ToDomain();
        await _abyssService.SaveContentAsync(processedContent);
        _logger.LogInformation("New content saved. Content id: {Id}", processedContent.Id);
    }
}