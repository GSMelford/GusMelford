using ContentCollector.MircoService.Domain.ContentProviders;

namespace ContentCollector.Services.ContentProviders;

public class ContentProviderService : IContentProviderService
{
    public ContentProvider Define(string message)
    {
        if (message.Contains(nameof(ContentProvider.TikTok).ToLower()))
        {
            return ContentProvider.TikTok;
        }

        return ContentProvider.Unknown;
    }
}