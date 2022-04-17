using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content;

public class ContentRepository : IContentRepository
{
    private readonly IDatabaseManager _databaseManager;
    
    public ContentRepository(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
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
}