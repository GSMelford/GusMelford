using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.DAL;
using GusMelfordBot.DAL.Applications.ContentCollector;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Contents.ContentProviders.TikTok;

public class TikTokRepository : ITikTokRepository
{
    private readonly IDatabaseManager _databaseManager;
    
    public TikTokRepository(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public TEntity? FirstOrDefault<TEntity>(Func<TEntity, bool> predicate) where TEntity : DatabaseEntity
    {
        return _databaseManager.Context.Set<TEntity>().FirstOrDefault(predicate);
    }

    public async Task<Chat?> GetChatAsync(long chatId)
    {
        return await _databaseManager.Context
            .Set<Chat>()
            .FirstOrDefaultAsync(x => x.ChatId == chatId && x.ApplicationType == "ContentCollector");
    }
    
    public async Task<User?> GetUserAsync(long telegramUserId)
    {
        return await _databaseManager.Context
            .Set<User>()
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);
    }

    public async Task<Content?> GetContentAsync(Guid contentId)
    {
        return await _databaseManager.Context
            .Set<Content>()
            .Include(x=>x.User)
            .Include(x=>x.Chat)
            .FirstOrDefaultAsync(x => x.Id == contentId);
    }

    public async Task<Content?> GetContentAsync(string refererLink)
    {
        return await _databaseManager.Context
            .Set<Content>()
            .FirstOrDefaultAsync(x => x.RefererLink == refererLink);
    }

    public async Task<int> GetCountAsync()
    {
        return await _databaseManager.Context.Set<Content>().CountAsync();
    }

    public async Task UpdateAndSaveContentAsync(Content? content)
    {
        if (content is not null)
        {
            _databaseManager.Context.Update(content);
            await _databaseManager.Context.SaveChangesAsync();
        }
    }
    
    public async Task AddAndSaveContentAsync(Content content)
    {
        await _databaseManager.Context.AddAsync(content);
        await _databaseManager.Context.SaveChangesAsync();
    }
}