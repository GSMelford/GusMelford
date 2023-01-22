namespace GusMelfordBot.Domain.Telegram;

public class TelegramCommand
{
    public string Name { get; set; } = null!;
    public List<string> Arguments { get; set; } = new ();
    public long TelegramId { get; set; }
    public long ChatId { get; set; }
}