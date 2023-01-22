using ContentProcessor.Worker.Domain.ContentProviders.TikTok;

namespace ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

public abstract class AbstractTikTokContentHandler : IHandler
{
    private IHandler? _nextHandler;
    
    public IHandler SetNext(IHandler handler)
    { 
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent processTikTokContent)
    {
        if (_nextHandler is not null)
        {
            return await _nextHandler.HandleAsync(processTikTokContent);
        }
        
        return processTikTokContent;
    }
}