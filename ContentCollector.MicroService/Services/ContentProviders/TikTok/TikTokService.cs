using ContentCollector.Domain.ContentProviders;
using ContentCollector.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Extensions;
using GusMelfordBot.Extensions.Services.DataLake;
using GusMelfordBot.SimpleKafka.Interfaces;

namespace ContentCollector.Services.ContentProviders.TikTok;

public class TikTokService : ITikTokService
{
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly IDataLakeService _dataLakeService;
    private readonly ILogger<ITikTokService> _logger;

    public TikTokService(
        IKafkaProducer<string> kafkaProducer, 
        IDataLakeService dataLakeService, 
        ILogger<ITikTokService> logger)
    {
        _kafkaProducer = kafkaProducer;
        _dataLakeService = dataLakeService;
        _logger = logger;
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
            .SetNext(new SaveHandler(_dataLakeService, _logger));
        
        processedContent = (await handler.Handle(processedContent)).IfNullThrow();
        await _kafkaProducer.ProduceAsync(processedContent.ToContentProcessedEvent());
    }
}