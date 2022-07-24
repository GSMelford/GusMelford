namespace ContentCollector.MircoService.Domain.ContentProviders.TikTok;

public interface ITikTokService
{
    Task Process(ProcessedContent? processedContent);
}