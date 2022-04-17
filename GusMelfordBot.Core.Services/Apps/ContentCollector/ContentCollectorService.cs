using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector;

public class ContentCollectorService : IContentCollectorService
{
    private readonly ITikTokService _tikTokService;
    
    public ContentCollectorService(ITikTokService tikTokService)
    {
        _tikTokService = tikTokService;
    }
    
    public void ProcessMessage(Message message)
    {
        string text = message.Text;

        if (text.Contains(nameof(ContentProvider.TikTok).ToLower()))
        {
            _tikTokService.ProcessMessage(message);
        }
    }
}