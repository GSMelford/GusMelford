namespace GusMelfordBot.Infrastructure.Models;

public class TelegramChat : BaseEntity<Guid>
{
    public Application Application { get; set; }
    public long ChatId { get; set; }
}