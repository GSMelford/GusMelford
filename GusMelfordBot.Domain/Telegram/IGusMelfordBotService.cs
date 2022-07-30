namespace GusMelfordBot.Domain.Telegram;

public interface IGusMelfordBotService
{
    Task DeleteMessage(long chatId, int messageId);
}