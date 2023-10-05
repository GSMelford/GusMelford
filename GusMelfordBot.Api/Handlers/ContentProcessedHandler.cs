using GusMelfordBot.Api.Services.Features.Abyss;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;
using GusMelfordBot.SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.Handlers;

public class ContentProcessedHandler : IEventHandler<ContentProcessedEvent>
{
    private readonly ILogger<IEventHandler<ContentProcessedEvent>> _logger;
    private readonly IAbyssService _abyssService;

    public ContentProcessedHandler(ILogger<IEventHandler<ContentProcessedEvent>> logger,IAbyssService abyssService)
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