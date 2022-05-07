using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.DAL.Applications.ContentCollector;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Services.Modules;

public class RefreshContent : IHostedService, IDisposable
{
    private int _executionCount;
    private readonly ILogger<RefreshContent> _logger;
    private Timer _timer = null!;
    private readonly IContentRepository _contentRepository;
    private readonly ITikTokService _tikTokService;
    
    public RefreshContent(
        ILogger<RefreshContent> logger, 
        IContentRepository contentRepository, 
        ITikTokService tikTokService)
    {
        _logger = logger;
        _contentRepository = contentRepository;
        _tikTokService = tikTokService;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RefreshContent Hosted Service running");
        
        _timer = new Timer(Refresh, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
        
        return Task.CompletedTask;
    }

    private async void Refresh(object? state)
    {
        var count = Interlocked.Increment(ref _executionCount);

        _logger.LogInformation("RefreshContent Hosted Service is working. Count: {Count}", count);
        
        List<Content> contents = _contentRepository.GetUnfinishedContents().ToList();
        int counter = 0;
        foreach (var content in contents)
        {
            switch (content.ContentProvider)
            {
                case nameof(ContentProvider.TikTok):
                    bool isUpdated = await _tikTokService.PullAndUpdateContentAsync(content.Id, content.Chat.ChatId);
                    if (isUpdated)
                    {
                        counter++;
                    }
                    
                    await Task.Delay(2000);
                    break;
            }
        }
        
        _logger.LogInformation("RefreshContent Hosted updated. " +
                               "For update: {Count For update}, updated {Updated Count}", contents.Count, counter);
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RefreshContent Hosted Service is stopping");

        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}