namespace GusMelford.TelegramBot.Domain.Telegram;

public class UpdateDomain
{
    public int UpdateId { get; }
    public MessageDomain? Message { get; }

    public UpdateDomain(int updateId, MessageDomain? message)
    {
        UpdateId = updateId;
        Message = message;
    }
}