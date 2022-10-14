using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Infrastructure.Repositories.Telegram;

public class CommandRepository : ICommandRepository
{
    private readonly IDatabaseContext _databaseContext;

    public CommandRepository(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<TelegramUserDomain> GetUser(long telegramId)
    {
        return (await _databaseContext
            .Set<TelegramUser>()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId))
            .ToDomain();
    }
}