using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Exception;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload;

public class ContentDownloadService : IContentDownloadService
{
    private readonly ILogger<ContentDownloadService> _logger;
    private readonly IContentDownloadRepository _contentDownloadRepository;
    private readonly IRequestService _requestService;
    private readonly IFtpServerService _ftpServerService;

    public ContentDownloadService(
        ILogger<ContentDownloadService> logger, 
        IContentDownloadRepository contentDownloadRepository, 
        IRequestService requestService, 
        IFtpServerService ftpServerService)
    {
        _logger = logger;
        _contentDownloadRepository = contentDownloadRepository;
        _requestService = requestService;
        _ftpServerService = ftpServerService;
    }

    public async Task<MemoryStream?> GetFileStreamContent(Guid contentId)
    {
        _logger.LogInformation("Content Request id {Id}", contentId);
        DAL.Applications.ContentCollector.Content? content = await _contentDownloadRepository.GetContent(contentId);

        if (content is null)
        {
            throw new WrongArgumentsException($"Content not found in database. Content Id {contentId}");
        }

        switch (content.ContentProvider)
        {
            case nameof(ContentProvider.TikTok):
                MemoryStream? memoryStream = await _ftpServerService.DownloadFile($"Contents/{content.Name}.mp4");
                if (memoryStream is not null)
                    return memoryStream;
                
                TikTokDownloadManager tikTokDownloadManager = new TikTokDownloadManager(_requestService, _logger);
                byte[]? bytes = await tikTokDownloadManager.DownloadTikTokVideo(content);
                if (bytes is null)
                    await _contentDownloadRepository.SetIsNotValid(contentId);
                return bytes is not null ? new MemoryStream(bytes) : null;
        }

        return null;
    }
}