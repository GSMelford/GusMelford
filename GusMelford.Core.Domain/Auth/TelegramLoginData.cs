namespace GusMelfordBot.Domain.Auth;

public class TelegramLoginData
{
    public long TelegramId { get; }
    public string Password { get; }

    public TelegramLoginData(long telegramId, string password)
    {
        TelegramId = telegramId;
        Password = password;
    }
}