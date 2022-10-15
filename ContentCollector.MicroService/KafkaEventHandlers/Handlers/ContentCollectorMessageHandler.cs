using ContentCollector.Domain.ContentProviders;
using ContentCollector.Domain.ContentProviders.TikTok;
using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.Services.ContentProviders.TikTok;
using GusMelfordBot.SimpleKafka.Interfaces;

namespace ContentCollector.KafkaEventHandlers.Handlers;

public class ContentCollectorMessageHandler : IEventHandler<ContentCollectorMessageEvent>
{
    private readonly ITikTokService _tikTokService;
    private readonly IKafkaProducer<string> _kafkaProducer;
    
    public ContentCollectorMessageHandler(IKafkaProducer<string> kafkaProducer, ITikTokService tikTokService)
    {
        _kafkaProducer = kafkaProducer;
        _tikTokService = tikTokService;
    }
    
    public Task Handle(ContentCollectorMessageEvent @event)
    {
        string messageText = @event.MessageText;
        switch (Define(messageText))
        {
            case ContentProvider.TikTok:
                _tikTokService.Process(TikTokServiceExtension.ToBasicProcessedContent(@event.Id, messageText)).Wait();
                return Task.CompletedTask;
            case ContentProvider.Unknown:
                //await _kafkaProducer.ProduceAsync(new ContentProcessedEvent { IsValid = false });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return Task.CompletedTask;
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