using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.DataLake;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.SimpleKafka.Interfaces;
using TBot.Client;
using TBot.Client.Api.Telegram.SendVideo;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class ContentProcessedHandler : IEventHandler<ContentProcessedEvent>
{
    private readonly ILogger<IEventHandler<ContentProcessedEvent>> _logger;
    private readonly IContentCollectorRepository _contentCollectorRepository;
    private readonly IContentCollectorRepository _collectorRepository;
    private readonly ITBot _tBot;
    private readonly IDataLakeService _dataLakeService;

    public ContentProcessedHandler(
        ILogger<IEventHandler<ContentProcessedEvent>> logger,
        IContentCollectorRepository contentCollectorRepository,
        IContentCollectorRepository collectorRepository,
        ITBot tBot, 
        IDataLakeService dataLakeService)
    {
        _logger = logger;
        _contentCollectorRepository = contentCollectorRepository;
        _collectorRepository = collectorRepository;
        _tBot = tBot;
        _dataLakeService = dataLakeService;
    }

    public async Task Handle(ContentProcessedEvent @event)
    {
        Guid contentId = await _contentCollectorRepository.Update(@event.ToContentProcessed());
        if (@event.IsSaved)
        {
            MemoryStream contentStream = new MemoryStream(await _dataLakeService.ReadFileAsync(@event.Path));
            await _tBot.SendVideoAsync(new SendVideoParameters
            {
                Caption = await _collectorRepository.GetVideoCaption(contentId),
                Video = new VideoFile(contentStream, contentId.ToString()),
                ChatId = await _collectorRepository.GetChatId(contentId) ?? default
            });
        }
    }
}