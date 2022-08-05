﻿using ContentCollector.Domain.ContentProviders;
using ContentCollector.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.DataLake;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class SaveHandler : AbstractTikTokContentHandler
{
    private readonly ILogger<ITikTokService> _logger;
    private readonly IDataLakeService _dataLakeService;
    
    public SaveHandler(IDataLakeService dataLakeService, ILogger<ITikTokService> logger)
    {
        _dataLakeService = dataLakeService;
        _logger = logger;
    }
    
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        processedTikTokContent.Path = processedTikTokContent.OriginalLink.BuildPathToContent();
        
        try {
            await _dataLakeService.WriteFile(processedTikTokContent.Path, processedTikTokContent.Bytes);
            processedTikTokContent.IsSaved = true;
        }
        catch (Exception exception) {
            _logger.LogError("Error while saving content. {ErrorMessage}", exception.Message);
            processedTikTokContent.IsSaved = false;
        }

        if (!processedTikTokContent.IsSaved)
        {
            return processedTikTokContent;
        }
        
        return await base.Handle(processedTikTokContent);
    }
}