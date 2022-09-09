namespace GusMelfordBot.Domain.Telegram;

public interface ILongCommandService
{
    LongCommand? GetLongCommand(long telegramId);
    bool TryAdd(long telegramId, string commandName);
    void Remove(long telegramId);
}