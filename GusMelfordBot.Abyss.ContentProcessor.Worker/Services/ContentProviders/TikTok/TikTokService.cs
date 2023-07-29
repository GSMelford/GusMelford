using System.Threading.Tasks;
using ContentProcessor.Worker.Abstractions;
using ContentProcessor.Worker.Domain;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers;
using ContentProcessor.Worker.Settings;
using GusMelfordBot.Extensions;
using GusMelfordBot.Extensions.Services.DataLake;
using Kyoto.Kafka.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContentProcessor.Worker.Services.ContentProviders.TikTok;

public class TikTokService : ITikTokService
{
    private readonly AppSettings _appSettings;
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly IDataLakeService _dataLakeService;
    private readonly ILogger<ITikTokService> _logger;

    public TikTokService(
        AppSettings appSettings,
        IKafkaProducer<string> kafkaProducer, 
        IDataLakeService dataLakeService, 
        ILogger<ITikTokService> logger)
    {
        _appSettings = appSettings;
        _kafkaProducer = kafkaProducer;
        _dataLakeService = dataLakeService;
        _logger = logger;
    }

    public async Task ProcessAsync(ProcessTikTokContent? processedContent)
    {
        if (processedContent is null) {
            return;
        }

        AbstractHandler<ProcessTikTokContent> handler = new RefererLinkHandler();
        handler
            .SetNext(new VideoInformationHandler())
            .SetNext(new DownloadLinkHandler())
            .SetNext(new SaveHandler(_dataLakeService, _logger));
        
        processedContent = (await handler.HandleAsync(processedContent)).IfNullThrow();

        if (processedContent is { IsSaved: false } content && content.Attempt <= _appSettings.Attempt)
        {
            await _kafkaProducer.ProduceAsync(processedContent.ToAttemptContentEvent(), string.Empty);
            return;
        }

        if (processedContent.IsAvailable) {
            await _kafkaProducer.ProduceAsync(processedContent.ToContentProcessedEvent(), string.Empty);
        }
    }
}