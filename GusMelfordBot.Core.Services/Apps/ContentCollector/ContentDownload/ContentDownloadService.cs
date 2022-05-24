using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;
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

    public ContentDownloadService(
        ILogger<ContentDownloadService> logger, 
        IContentDownloadRepository contentDownloadRepository,
        IFtpServerService ftpServerService)
    {
        _logger = logger;
        _contentDownloadRepository = contentDownloadRepository;
        _ftpServerService = ftpServerService;
    }

    public async Task<MemoryStream?> GetFileStreamContent(Guid contentId)
    {
        _logger.LogInformation("Content Request id {Id}", contentId);
        Content? content = await _contentDownloadRepository.GetContent(contentId);

        if (content is null)
        {
            throw new WrongArgumentsException($"Content not found in database. Content Id {contentId}");
        }

        return content.ContentProvider switch
        {
            nameof(ContentProvider.TikTok) => await _ftpServerService.DownloadFile($"contents/{content.Name}.mp4"),
            _ => null
        };
    }
}