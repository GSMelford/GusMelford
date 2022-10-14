using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Infrastructure.Models;

namespace GusMelfordBot.Infrastructure.Repositories.Telegram;

public static class Convert
{
    public static TelegramUserDomain ToDomain(this TelegramUser? telegramUser)
    {
        return new TelegramUserDomain(
            telegramUser.User.Id, 
            telegramUser.TelegramId, 
            telegramUser.User.FirstName,
            telegramUser.User.LastName,
            telegramUser.Username, 
            telegramUser.User.Password);
    }
}