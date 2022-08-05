using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class RefererLinkHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        if (processedTikTokContent.OriginalLink.Contains("https://www.tiktok.com/@")) {
            return await base.Handle(processedTikTokContent);
        }
        
        string? refererLink =
            (await processedTikTokContent.OriginalLink.GetRefererLink() ?? string.Empty).Split("?").FirstOrDefault();
        
        if (string.IsNullOrEmpty(refererLink)) {
            return processedTikTokContent;
        }

        processedTikTokContent.OriginalLink = refererLink;
        return await base.Handle(processedTikTokContent);
    }
}