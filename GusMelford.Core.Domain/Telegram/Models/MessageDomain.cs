namespace GusMelfordBot.Domain.Telegram.Models;

public class MessageDomain
{
    public int? MessageId { get; }
    public string? Text { get; }
    public string? Caption { get; }
    public TelegramObjectUserDomain? From { get; }
    public ChatDomain? Chat { get; }

    public MessageDomain(int? messageId, string? text, string? caption, TelegramObjectUserDomain? from, ChatDomain? chat)
    {
        MessageId = messageId;
        Text = text;
        Caption = caption;
        From = from;
        Chat = chat;
    }
}