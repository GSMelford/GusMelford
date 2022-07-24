using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.MircoService.Domain.System;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Extensions;
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

    public async Task Process(ProcessedContent? processedContent)
    {
        if (processedContent is null) {
            return;
        }

        var handler = new RefererLinkHandler();
        handler
            .SetNext(new VideoInformationHandler())
            .SetNext(new ValidVideoHandler())
            .SetNext(new DownloadLinkHandler())
            .SetNext(new UploadToFtpHandler(_ftpServerService));
        
        processedContent = (await handler.Handle(processedContent)).IfNullThrow();
        await _kafkaProducer.ProduceAsync(processedContent.ToContentProcessedEvent());
    }
}