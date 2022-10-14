using GusMelford.TelegramBot.Domain.Telegram;

namespace GusMelford.TelegramBot.Services.Update.MessageHandlers.Abstractions;

public class AbstractMessageHandler
{
    private IHandler? _nextHandler;
    
    public IHandler SetNext(IHandler handler)
    { 
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<MessageDomain?> Handle(MessageDomain messageDomain)
    {
        if (_nextHandler is not null)
        {
            return await _nextHandler.Handle(messageDomain);
        }
        
        return messageDomain;
    }
}