using System.Threading.Tasks;
using ContentProcessor.Worker.Domain;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using GusMelfordBot.Events;
using Kyoto.Kafka.Interfaces;

namespace ContentProcessor.Worker.KafkaHandlers;

public class ContentHandler : IKafkaHandler<ContentEvent>
{
    private readonly ITikTokService _tikTokService;
    
    public ContentHandler(ITikTokService tikTokService)
    {
        _tikTokService = tikTokService;
    }
    
    public async Task HandleAsync(ContentEvent @event)
    {
        string messageText = @event.Message;
        switch (DefineContentProvider(messageText))
        {
            case ContentProvider.TikTok:
                await _tikTokService.ProcessAsync(@event.ToDomain());
                break;
            case ContentProvider.Unknown:
                break;
        }
    }
    
    private static ContentProvider DefineContentProvider(string message)
    {
        return message.Contains(nameof(ContentProvider.TikTok).ToLower()) 
            ? ContentProvider.TikTok 
            : ContentProvider.Unknown;
    }
}