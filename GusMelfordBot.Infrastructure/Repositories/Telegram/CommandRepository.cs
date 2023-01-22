using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using GusMelfordBot.Infrastructure.Repositories.Auth;
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
        return null;
    }
}