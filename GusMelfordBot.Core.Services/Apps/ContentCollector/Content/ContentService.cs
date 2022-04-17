using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content;

public class ContentService : IContentService
{
    private readonly ILogger<ContentService> _logger;
    private readonly IContentRepository _contentRepository;
    
    public ContentService(
        ILogger<ContentService> logger,
        IContentRepository contentRepository)
    {
        _logger = logger;
        _contentRepository = contentRepository;
    }

    public List<ContentInfo> BuildContentInfoList(Filter filter)
    {
        _logger.LogInformation("Request for content. Chat Id: {ChatId}", filter.ChatId);
        return _contentRepository.GetContentList(filter).ToList();
    }
    
    public async Task SetViewedVideo(Guid contentId)
    {
        await _contentRepository.SetViewedVideo(contentId);
    }
}