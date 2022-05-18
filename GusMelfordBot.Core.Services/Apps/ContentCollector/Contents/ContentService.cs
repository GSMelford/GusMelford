using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.DAL.Applications.ContentCollector;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Contents;

public class ContentService : IContentService
{
    private readonly ILogger<ContentService> _logger;
    private readonly IContentRepository _contentRepository;
    private readonly ITikTokService _tikTokService;
    
    public ContentService(
        ILogger<ContentService> logger,
        IContentRepository contentRepository,
        ITikTokService tikTokService)
    {
        _logger = logger;
        _contentRepository = contentRepository;
        _tikTokService = tikTokService;
    }

    public List<ContentInfoDomain> BuildContentInfoList(Filter filter)
    {
        _logger.LogInformation("Request for content. Chat Id: {ChatId}", filter.ChatId);
        return _contentRepository.GetContentList(filter).ToList();
    }
    
    public async Task SetViewedVideo(Guid contentId)
    {
        await _contentRepository.SetViewedVideo(contentId);
    }

    public async Task<int> Refresh(long chatId)
    {
        List<Content> contents = _contentRepository.GetUnfinishedContents().ToList();
        foreach (var content in contents)
        {
            switch (content.ContentProvider)
            {
                case nameof(ContentProvider.TikTok):
                    await _tikTokService.PullAndUpdateContentAsync(content.Id, chatId);
                    await Task.Delay(2000);
                    break;
            }
        }

        return contents.Count;
    }
}