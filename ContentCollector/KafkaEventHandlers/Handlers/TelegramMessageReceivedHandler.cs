using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.Services;
using ContentCollector.Utilities;
using SimpleKafka.Interfaces;

namespace ContentCollector.KafkaEventHandlers.Handlers;

public class TelegramMessageReceivedHandler : IEventHandler<TelegramMessageReceivedEvent>
{
    private readonly ContentProviderService _contentProviderService;
    
    public TelegramMessageReceivedHandler(ContentProviderService contentProviderService)
    {
        _contentProviderService = contentProviderService;
    }
    
    public Task Handle(TelegramMessageReceivedEvent @event)
    {
        switch (_contentProviderService.Define(@event.Message))
        {
            case ContentProvider.TikTok:
                break;
            case ContentProvider.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return Task.CompletedTask;
    }
}