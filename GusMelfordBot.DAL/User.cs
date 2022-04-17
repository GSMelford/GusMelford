namespace GusMelfordBot.DAL;

public class User : DatabaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public long TelegramUserId { get; set; }
}