namespace GusMelfordBot.Core.Settings;

public class TelegramBotSettings
{
    public TelegramBotSettings(string? token)
    {
        Token = token;
    }

    public string? Token { get; set; }
}