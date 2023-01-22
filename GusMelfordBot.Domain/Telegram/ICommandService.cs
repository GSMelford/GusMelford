namespace GusMelfordBot.Domain.Telegram;

public interface ICommandService
{
    bool IsCommand(string messageText);
    bool IsCommandInProgress(long telegramUserId);
    Task ExecuteAsync(long groupId, long telegramUserId, string input);
}