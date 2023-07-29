using System;
using System.IO;
using System.Threading.Tasks;
using ContentProcessor.Worker.Abstractions;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using GusMelfordBot.Domain;
using GusMelfordBot.Extensions.Services.DataLake;
using Microsoft.Extensions.Logging;

namespace ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class SaveHandler : AbstractHandler<ProcessTikTokContent>
{
    private readonly ILogger<ITikTokService> _logger;
    private readonly IDataLakeService _dataLakeService;
    
    public SaveHandler(IDataLakeService dataLakeService, ILogger<ITikTokService> logger)
    {
        _dataLakeService = dataLakeService;
        _logger = logger;
    }
    
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent processTikTokContent)
    {
        try {
            _dataLakeService.CreateDirectoryIfNotExist(Constants.ContentFolder);
            await _dataLakeService.WriteFile(Path.Combine(Constants.ContentFolder, $"{processTikTokContent.Id}.mp4"), processTikTokContent.Bytes);
            processTikTokContent.IsSaved = true;
        }
        catch (Exception exception) {
            _logger.LogError("Error while saving content. {ErrorMessage}", exception.Message);
            processTikTokContent.IsSaved = false;
        }

        if (!processTikTokContent.IsSaved) {
            return processTikTokContent.MarkAsProcessFailed();
        }
        
        return await base.HandleAsync(processTikTokContent);
    }
}