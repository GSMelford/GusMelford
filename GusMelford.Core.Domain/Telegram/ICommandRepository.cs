namespace GusMelfordBot.Domain.Telegram;

public interface ICommandRepository
{
    Task<TelegramUserDomain> GetUser(long telegramId);
}