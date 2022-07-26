using ContentCollector.Domain.ContentProviders;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

public interface IHandler
{
    IHandler SetNext(IHandler handler);
    Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent);
}