using GusMelfordBot.Core.Domain.Apps;
using GusMelfordBot.DAL;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Services.Apps;

public class ApplicationRepository : IApplicationRepository
{
    private readonly IDatabaseManager _databaseManager;
    
    public ApplicationRepository(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public async Task<List<Chat>> GetChats()
    {
        return await _databaseManager.Context.Set<Chat>().ToListAsync();
    }

    public async Task RegisterNewUserIfNotExist(global::Telegram.Dto.User userTelegram)
    {
        User? user = await _databaseManager.Context
            .Set<User>()
            .FirstOrDefaultAsync(x => x.TelegramUserId == userTelegram.Id);

        if (user is null)
        {
            user = new User
            {
                FirstName = userTelegram.FirstName,
                LastName = userTelegram.LastName,
                UserName = userTelegram.Username,
                TelegramUserId = userTelegram.Id
            };

            await _databaseManager.Context.AddAsync(user);
            await _databaseManager.Context.SaveChangesAsync();
        }
    }
}