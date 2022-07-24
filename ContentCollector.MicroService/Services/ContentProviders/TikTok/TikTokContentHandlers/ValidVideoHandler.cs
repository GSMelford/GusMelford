using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class ValidVideoHandler : AbstractTikTokContentHandler
{
    public override async Task<ProcessedContent?> Handle(ProcessedContent processedContent)
    {
        if (processedContent.VideoStatusCode is not (<= 10000 or >= 11000))
        {
            return processedContent;
        }

        processedContent.IsValid = true;
        return await base.Handle(processedContent);
    }
}