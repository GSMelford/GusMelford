namespace GusMelfordBot.Domain.Telegram.Models;

public class UpdateDomain
{
    public long UpdateId { get; }
    public MessageDomain? Message { get; }

    public UpdateDomain(long updateId, MessageDomain? message)
    {
        UpdateId = updateId;
        Message = message;
    }
}