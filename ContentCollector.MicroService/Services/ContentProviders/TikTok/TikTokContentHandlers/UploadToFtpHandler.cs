using ContentCollector.Domain.ContentProviders;
using ContentCollector.Domain.System;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class UploadToFtpHandler : AbstractTikTokContentHandler
{
    private readonly IFtpServerService _ftpServerService;
    
    public UploadToFtpHandler(IFtpServerService ftpServerService)
    {
        _ftpServerService = ftpServerService;
    }
    
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        processedTikTokContent.Path = processedTikTokContent.OriginalLink.BuildPathToContent();
        bool isSuccessfullySaved =
            await _ftpServerService.UploadFile(processedTikTokContent.Path, new MemoryStream(processedTikTokContent.Bytes));

        if (!isSuccessfullySaved)
        {
            return processedTikTokContent;
        }
        
        return await base.Handle(processedTikTokContent);
    }
}