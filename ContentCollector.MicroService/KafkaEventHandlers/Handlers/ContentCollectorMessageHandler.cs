using ContentCollector.Domain.ContentProviders;
using ContentCollector.Domain.ContentProviders.TikTok;
using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.Services.ContentProviders.TikTok;
using SimpleKafka.Interfaces;

namespace ContentCollector.KafkaEventHandlers.Handlers;

public class ContentCollectorMessageHandler : IEventHandler<ContentCollectorMessageEvent>
{
    private readonly ITikTokService _tikTokService;
    
    public ContentCollectorMessageHandler(ITikTokService tikTokService)
    {
        _tikTokService = tikTokService;
    }
    
    public async Task Handle(ContentCollectorMessageEvent @event)
    {
        string messageText = @event.MessageText;
        switch (Define(messageText))
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
    
    private static ContentProvider Define(string message)
    {
        if (message.Contains(nameof(ContentProvider.TikTok).ToLower()))
        {
            return ContentProvider.TikTok;
        }

        return ContentProvider.Unknown;
    }
}