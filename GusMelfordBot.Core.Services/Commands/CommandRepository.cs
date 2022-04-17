using GusMelfordBot.Core.Domain.Commands;
using GusMelfordBot.Core.Services.Apps;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Dto;

namespace GusMelfordBot.Core.Services.Commands;

public class CommandRepository : ICommandRepository
{
    private readonly IDatabaseManager _databaseManager;
    
    public CommandRepository(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public async Task<bool> RegisterContentCollectorGroup(Chat chat)
    {
        DAL.Chat? chatBase = await _databaseManager.Context
            .Set<DAL.Chat>()
            .FirstOrDefaultAsync(x => x.ChatId == chat.Id);

        if (chatBase is not null)
        {
             return false;
        }
        
        chatBase = new DAL.Chat
        {
            ApplicationType = App.ContentCollector,
            ChatId = chat.Id
        };

        await _databaseManager.Context.AddAsync(chatBase);
        await _databaseManager.Context.SaveChangesAsync();
        return true;
    }
}