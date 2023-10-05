using ContentProcessor.Worker.Domain.ContentProviders.TikTok;

namespace ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

public interface IHandler
{
    IHandler SetNext(IHandler handler);
    Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent processTikTokContent);
}