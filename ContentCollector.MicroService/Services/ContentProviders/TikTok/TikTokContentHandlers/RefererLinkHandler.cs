using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class RefererLinkHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedContent?> Handle(ProcessedContent processedContent)
    {
        if (processedContent.OriginalLink.Contains("https://www.tiktok.com/@"))
        {
            return await base.Handle(processedContent);
        }
        
        string? refererLink = await processedContent.OriginalLink.GetRefererLink();
        if (string.IsNullOrEmpty(refererLink))
        {
            return processedContent;
        }

        processedContent.OriginalLink = refererLink;
        return await base.Handle(processedContent);
    }
}