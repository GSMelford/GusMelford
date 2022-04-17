using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Exception;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload;

public class ContentDownloadService : IContentDownloadService
{
    private readonly ILogger<ContentDownloadService> _logger;
    private readonly IContentDownloadRepository _contentDownloadRepository;
    private readonly IRequestService _requestService;

    public ContentDownloadService(
        ILogger<ContentDownloadService> logger, 
        IContentDownloadRepository contentDownloadRepository, 
        IRequestService requestService)
    {
        _logger = logger;
        _contentDownloadRepository = contentDownloadRepository;
        _requestService = requestService;
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
                TikTokDownloadManager tikTokDownloadManager = new TikTokDownloadManager(_requestService, _logger);
                byte[]? bytes = await tikTokDownloadManager.DownloadTikTokVideo(content);
                if (bytes is null)
                    await _contentDownloadRepository.SetIsNotValid(contentId);
                return bytes is not null ? new MemoryStream(bytes) : null;
        }

        return null;
    }
}