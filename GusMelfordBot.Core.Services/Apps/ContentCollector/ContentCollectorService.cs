using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector;

public class ContentCollectorService : IContentCollectorService
{
    private readonly ITikTokService _tikTokService;
    private readonly IContentService _contentService;
    
    public ContentCollectorService(ITikTokService tikTokService, IContentService contentService)
    {
        _tikTokService = tikTokService;
        _contentService = contentService;
    }

    public async Task ProcessMessage(Message message)
    {
        string text = message.Text;

        if (text.Contains(nameof(ContentProvider.TikTok).ToLower()))
        {
            await _tikTokService.ProcessMessageAsync(message);
        }
    }

    public void ProcessCallbackQuery(CallbackQuery callbackQuery)
    {
        throw new NotImplementedException();
    }

    public int Refresh(long chatId)
    {
        return _contentService.Refresh(chatId);
    }
}