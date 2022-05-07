using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Exception;
using GusMelfordBot.DAL.Applications.ContentCollector;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload;

public class ContentDownloadService : IContentDownloadService
{
    private readonly ILogger<ContentDownloadService> _logger;
    private readonly IContentDownloadRepository _contentDownloadRepository;
    private readonly IFtpServerService _ftpServerService;
    private readonly ITikTokDownloaderService _tikTokDownloaderService;

    public ContentDownloadService(
        ILogger<ContentDownloadService> logger, 
        IContentDownloadRepository contentDownloadRepository,
        IFtpServerService ftpServerService,
        ITikTokDownloaderService tikTokDownloaderService)
    {
        _logger = logger;
        _contentDownloadRepository = contentDownloadRepository;
        _ftpServerService = ftpServerService;
        _tikTokDownloaderService = tikTokDownloaderService;
    }

    public async Task<MemoryStream?> GetFileStreamContent(Guid contentId)
    {
        _logger.LogInformation("Content Request id {Id}", contentId);
        Content? content = await _contentDownloadRepository.GetContent(contentId);

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

                if (!await _tikTokDownloaderService.TryGetAndSaveRefererLink(content))
                {
                    return null;
                }
                
                byte[]? bytes = await _tikTokDownloaderService.DownloadTikTokVideo(content);
                if (bytes is null)
                    await _contentDownloadRepository.SetIsNotValid(contentId);
                return bytes is not null ? new MemoryStream(bytes) : null;
        }

        return null;
    }
}