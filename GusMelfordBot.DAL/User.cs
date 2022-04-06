namespace GusMelfordBot.DAL;

public class User : DatabaseEntity
{
    public long TelegramUserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
}