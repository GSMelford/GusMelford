using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.DAL;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders.TikTok;

public class TikTokRepository : ITikTokRepository
{
    private readonly IDatabaseManager _databaseManager;
    
    public TikTokRepository(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
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

    public async Task<DAL.Applications.ContentCollector.Content?> GetContentAsync(string refererLink)
    {
        return await _databaseManager.Context
            .Set<DAL.Applications.ContentCollector.Content>()
            .FirstOrDefaultAsync(x => x.RefererLink == refererLink);
    }

    public async Task<int> GetCountAsync()
    {
        return await _databaseManager.Context.Set<DAL.Applications.ContentCollector.Content>().CountAsync();
    }

    public async Task SaveContentAsync(DAL.Applications.ContentCollector.Content content)
    {
        await _databaseManager.Context.AddAsync(content);
        await _databaseManager.Context.SaveChangesAsync();
    }
}