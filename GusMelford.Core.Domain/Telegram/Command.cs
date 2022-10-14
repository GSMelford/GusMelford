namespace GusMelfordBot.Domain.Telegram;

public class Command
{
    public string Name { get; set; } = null!;
    public List<string> Arguments { get; set; } = new ();
    public long TelegramId { get; set; }
    public long ChatId { get; set; }
    public bool IsLongCommandActive { get; set; }
}