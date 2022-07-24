namespace GusMelfordBot.Domain.Telegram.Models;

public class ChatDomain
{
    public long? Id { get; }

    public ChatDomain(long? id)
    {
        Id = id;
    }
}