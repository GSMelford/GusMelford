using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Exception;
using GusMelfordBot.DAL;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Services.System;

public class SystemRepository : ISystemRepository
{
    private readonly IDatabaseManager _databaseManager;
    
    public SystemRepository(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public async Task<bool> CheckCredentials(int telegramId, string? password)
    {
        User? user = await _databaseManager.Context
            .Set<User>()
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramId && x.Password == password);
        return user is not null;
    }

    public async Task AddUser(UserDomain userDomain)
    {
        User? userDb = await _databaseManager.Context
            .Set<User>()
            .FirstOrDefaultAsync(x => x.TelegramUserId == userDomain.TelegramUserId);

        if (userDb is not null)
        {
            throw new WrongArgumentsException(
                $"A user with such a telegram id \"{userDomain.TelegramUserId}\" exists");
        }

        await _databaseManager.Context.AddAsync(new User
        {
            FirstName = userDomain.FirstName,
            LastName = userDomain.LastName,
            UserName = userDomain.UserName,
            TelegramUserId = userDomain.TelegramUserId
        });
        await _databaseManager.Context.SaveChangesAsync();
    }
}