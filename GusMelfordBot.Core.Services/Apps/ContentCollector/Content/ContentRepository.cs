using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;
using GusMelfordBot.DAL;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content;

public class ContentRepository : IContentRepository
{
    private readonly IDatabaseManager _databaseManager;
    private readonly IFtpServerService _ftpServerService;
    private readonly ITikTokDownloaderService _tikTokDownloaderService;
    
    public ContentRepository(
        IDatabaseManager databaseManager,
        IFtpServerService ftpServerService,
        ITikTokDownloaderService tikTokDownloaderService)
    {
        _databaseManager = databaseManager;
        _ftpServerService = ftpServerService;
        _tikTokDownloaderService = tikTokDownloaderService;
    }

    public async Task<long?> GetChatId(Guid chatId)
    {
        return (await _databaseManager.Context.Set<Chat>().FirstOrDefaultAsync(x => x.Id == chatId))?.ChatId;
    }
    
    public async Task<DAL.Applications.ContentCollector.Content?> GetContent(Guid contentId)
    {
        return (await _databaseManager.Context
            .Set<DAL.Applications.ContentCollector.Content>()
            .Include(x=>x.Chat)
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x => x.Id == contentId));
    }
    
    public IEnumerable<ContentInfo> GetContentList(Filter filter)
    {
        IQueryable<DAL.Applications.ContentCollector.Content> query = _databaseManager.Context
            .Set<DAL.Applications.ContentCollector.Content>()
            .Include(x => x.User)
            .Include(x => x.Chat)
            .Where(x => 
                x.IsViewed == filter.IsNotViewed 
                && x.Chat.Id == filter.ChatId
                && x.IsValid == true);

        if (filter.SinceDateTime is not null)
        {
            query = query.Where(x => x.CreatedOn > filter.SinceDateTime);
        }
        
        if (filter.UntilDateTime is not null)
        {
            query = query.Where(x => x.CreatedOn < filter.UntilDateTime);
        }

        if (filter.ContentProviders is not null)
        {
            query = filter.ContentProviders
                .Aggregate(query, (current, contentProvider) => 
                    current.Where(x => x.ContentProvider.Equals(contentProvider)));
        }

        foreach (var entity in query)
        {
            yield return entity.ToDomain();
        }
    }

    public async Task SetViewedVideo(Guid contentId)
    {
        var content = await _databaseManager.Context
            .Set<DAL.Applications.ContentCollector.Content>()
            .FirstOrDefaultAsync(x => x.Id == contentId);
        if (content is null)
        {
            return;
        }

        content.IsViewed = true;
        _databaseManager.Context.Update(content);
        await _databaseManager.Context.SaveChangesAsync();
    }

    public async Task Cache()
    {
        var contents = _databaseManager.Context
            .Set<DAL.Applications.ContentCollector.Content>()
            .Where(x => x.IsSaved == false).ToList();
        
        foreach (var content in contents)
        {
            switch (content.ContentProvider)
            {
                case nameof(ContentProvider.TikTok):
                {
                    byte[]? contentByte = await _tikTokDownloaderService.DownloadTikTokVideo(content);
                    if (contentByte != null)
                    {
                        MemoryStream memoryStream = new MemoryStream(contentByte);
                        string userName = TikTokServiceHelper.GetUserName(content.RefererLink);
                        string videoId = TikTokServiceHelper.GetVideoId(content.RefererLink);
                        string videoName = $"{userName}-{videoId}";
                
                        content.Name = videoName;
                        
                        content.IsSaved = await _ftpServerService
                            .UploadFile($"Contents/{content.Name}.mp4", memoryStream);
                        await Task.Delay(1000);
                    }
                    break;
                }
            }

            await _databaseManager.Context.SaveChangesAsync();
        }
    }
}