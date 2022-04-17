namespace GusMelfordBot.DAL;

public class Chat : DatabaseEntity
{
    public long ChatId { get; set; }
    public string ApplicationType { get; set; }
}