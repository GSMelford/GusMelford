using ContentCollector.Utilities;

namespace ContentCollector.Services;

public class ContentProviderService
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