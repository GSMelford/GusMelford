﻿namespace ContentProcessor.Worker.Domain.ContentProviders.TikTok;

public interface ITikTokService
{
    Task ProcessAsync(ProcessTikTokContent? processedContent);
}