using GusMelford.TelegramBot.Domain.Telegram;

namespace GusMelford.TelegramBot.Services.Update.MessageHandlers.Abstractions;

public interface IHandler
{
    IHandler SetNext(IHandler handler);
    Task<MessageDomain?> Handle(MessageDomain messageDomain);
}