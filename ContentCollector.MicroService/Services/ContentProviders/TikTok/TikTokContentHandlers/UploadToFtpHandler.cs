using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.MircoService.Domain.System;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class UploadToFtpHandler : AbstractTikTokContentHandler
{
    private readonly IFtpServerService _ftpServerService;
    
    public UploadToFtpHandler(IFtpServerService ftpServerService)
    {
        _ftpServerService = ftpServerService;
    }
    
    public override async Task<ProcessedContent?> Handle(ProcessedContent processedContent)
    {
        processedContent.Path = processedContent.OriginalLink.BuildPathToContent();
        bool isSuccessfullySaved =
            await _ftpServerService.UploadFile(processedContent.Path, new MemoryStream(processedContent.Bytes));

        if (!isSuccessfullySaved)
        {
            return processedContent;
        }
        
        return await base.Handle(processedContent);
    }
}