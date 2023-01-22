namespace GusMelfordBot.Domain.Application.ContentCollector;

public class AbyssContext
{
    public Guid SessionId { get; }
    public string Message { get; }
    public long TelegramUserId { get; }
    public long TelegramChatId { get; }
    public int TelegramMessageId { get; }

    public AbyssContext(Guid sessionId, string message, long telegramUserId, long telegramChatId, int telegramMessageId)
    {
        SessionId = sessionId;
        Message = message;
        TelegramUserId = telegramUserId;
        TelegramChatId = telegramChatId;
        TelegramMessageId = telegramMessageId;
    }
}