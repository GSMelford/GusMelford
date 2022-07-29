using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Extensions.Services.Ftp;

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

        processedTikTokContent.IsSaved = isSuccessfullySaved;
        return await base.Handle(processedTikTokContent);
    }
}