using ContentCollector.MircoService.Domain.ContentProviders.TikTok;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

public interface IHandler
{
    IHandler SetNext(IHandler handler);
    Task<ProcessedContent?> Handle(ProcessedContent processedContent);
}