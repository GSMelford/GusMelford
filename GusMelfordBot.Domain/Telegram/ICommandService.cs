namespace GusMelfordBot.Domain.Telegram;

public interface ICommandService
{
    Task ExecuteAsync(Command command);
}