using ContentCollector.Domain.ContentProviders;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

public abstract class AbstractTikTokContentHandler : IHandler
{
    private IHandler? _nextHandler;
    
    public IHandler SetNext(IHandler handler)
    { 
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        return await _nextHandler?.Handle(processedTikTokContent)!;
    }
}