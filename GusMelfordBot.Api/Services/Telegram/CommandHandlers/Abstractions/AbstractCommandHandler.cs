using GusMelfordBot.Domain.Telegram;

namespace GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;

public class AbstractCommandHandler : IHandler
{
    private IHandler? _nextHandler;
    
    public IHandler SetNext(IHandler handler)
    { 
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<TelegramCommand> Handle(TelegramCommand telegramCommand)
    {
        if (_nextHandler is not null)
        {
            return await _nextHandler.Handle(telegramCommand);
        }
        
        return telegramCommand;
    }
}