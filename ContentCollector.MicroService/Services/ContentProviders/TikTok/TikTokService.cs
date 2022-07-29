using ContentCollector.Domain.ContentProviders;
using ContentCollector.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Extensions;
using GusMelfordBot.Extensions.Services.Ftp;
using SimpleKafka.Interfaces;

namespace ContentCollector.Services.ContentProviders.TikTok;

public class TikTokService : ITikTokService
{
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly IFtpServerService _ftpServerService;

    public TikTokService(IKafkaProducer<string> kafkaProducer, IFtpServerService ftpServerService)
    {
        _kafkaProducer = kafkaProducer;
        _ftpServerService = ftpServerService;
    }

    public async Task Process(ProcessedTikTokContent? processedContent)
    {
        if (processedContent is null) {
            return;
        }

        AbstractTikTokContentHandler handler = new RefererLinkHandler();
        handler
            .SetNext(new VideoInformationHandler())
            .SetNext(new ValidVideoHandler())
            .SetNext(new DownloadLinkHandler())
            .SetNext(new UploadToFtpHandler(_ftpServerService));
        
        processedContent = (await handler.Handle(processedContent)).IfNullThrow();
        await _kafkaProducer.ProduceAsync(processedContent.ToContentProcessedEvent());
    }
}