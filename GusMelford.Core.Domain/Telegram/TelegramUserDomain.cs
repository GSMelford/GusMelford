namespace GusMelfordBot.Domain.Telegram;

public class TelegramUserDomain
{
    public Guid Id { get; }
    public long TelegramId { get; }
    public string FirstName { get; }
    public string? LastName { get; }
    public string? Username { get; }
    public string? Password { get; }

    public TelegramUserDomain(
        Guid id, 
        long telegramId,
        string firstName, 
        string? lastName, 
        string? username, 
        string? password)
    {
        Id = id;
        TelegramId = telegramId;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        Password = password;
    }
}