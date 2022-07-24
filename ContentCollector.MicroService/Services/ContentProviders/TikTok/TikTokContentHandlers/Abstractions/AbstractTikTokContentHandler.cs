using ContentCollector.MircoService.Domain.ContentProviders.TikTok;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

public abstract class AbstractTikTokContentHandler : IHandler
{
    private IHandler? _nextHandler;
    
    public IHandler SetNext(IHandler handler)
    { 
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<ProcessedContent?> Handle(ProcessedContent processedContent)
    {
        return await _nextHandler?.Handle(processedContent)!;
    }
}