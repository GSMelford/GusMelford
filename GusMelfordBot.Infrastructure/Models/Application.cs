namespace GusMelfordBot.Infrastructure.Models;

public class Application : BaseEntity
{
    public string Name { get; set; }
    public ICollection<TelegramChat> TelegramChats { get; set; } = new List<TelegramChat>();
}