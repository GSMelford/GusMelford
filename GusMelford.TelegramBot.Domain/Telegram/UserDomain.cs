namespace GusMelford.TelegramBot.Domain.Telegram;

public class UserDomain
{
    public long Id { get; }
    public string FirstName { get; }
    public string? LastName { get; }
    public string? Username { get; }

    public UserDomain(long id, string firstName, string? lastName, string? username)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
    }
}