using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.MircoService.Domain.ContentProviders;
using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok;
using SimpleKafka.Interfaces;

namespace ContentCollector.KafkaEventHandlers.Handlers;

public class ContentCollectorMessageHandler : IEventHandler<ContentCollectorMessageEvent>
{
    private readonly IContentProviderService _contentProviderService;
    private readonly ITikTokService _tikTokService;
    
    public ContentCollectorMessageHandler(
        IContentProviderService contentProviderService,
        ITikTokService tikTokService)
    {
        _contentProviderService = contentProviderService;
        _tikTokService = tikTokService;
    }
    
    public async Task Handle(ContentCollectorMessageEvent @event)
    {
        string messageText = @event.MessageText;
        switch (_contentProviderService.Define(messageText))
        {
            case ContentProvider.TikTok:
                await _tikTokService.Process(TikTokServiceExtension.ToBasicProcessedContent(@event.Id, messageText));
                break;
            case ContentProvider.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}