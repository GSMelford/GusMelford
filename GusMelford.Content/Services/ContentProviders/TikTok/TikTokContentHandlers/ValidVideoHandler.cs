using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class ValidVideoHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        if (processedTikTokContent.VideoStatusCode is not (<= 10000 or >= 11000))
        {
            return processedTikTokContent;
        }

        processedTikTokContent.IsValid = true;
        return await base.Handle(processedTikTokContent);
    }
}