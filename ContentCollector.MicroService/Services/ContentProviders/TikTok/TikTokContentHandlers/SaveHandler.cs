using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.DataLake;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class SaveHandler : AbstractTikTokContentHandler
{
    private readonly IDataLakeService _dataLakeService;
    
    public SaveHandler(IDataLakeService dataLakeService)
    {
        _dataLakeService = dataLakeService;
    }
    
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        processedTikTokContent.Path = processedTikTokContent.OriginalLink.BuildPathToContent();
        
        try {
            await _dataLakeService.WriteFile(processedTikTokContent.Path, processedTikTokContent.Bytes);
            processedTikTokContent.IsSaved = true;
        }
        catch {
            processedTikTokContent.IsSaved = false;
        }

        if (!processedTikTokContent.IsSaved)
        {
            return processedTikTokContent;
        }
        
        return await base.Handle(processedTikTokContent);
    }
}