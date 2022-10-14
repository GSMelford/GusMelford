namespace ContentCollector.Domain.ContentProviders.TikTok;

public interface ITikTokService
{
    Task Process(ProcessedTikTokContent? processedContent);
}