using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Extensions.Services.Ftp;
using GusMelfordBot.SimpleKafka.Interfaces;
using TBot.Client;
using TBot.Client.Api.Telegram.SendVideo;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class ContentProcessedHandler : IEventHandler<ContentProcessedEvent>
{
    private readonly ILogger<IEventHandler<ContentProcessedEvent>> _logger;
    private readonly IContentCollectorRepository _contentCollectorRepository;
    private readonly IContentCollectorRepository _collectorRepository;
    private readonly IFtpServerService _ftpServerService;
    private readonly ITBot _tBot;

    public ContentProcessedHandler(
        ILogger<IEventHandler<ContentProcessedEvent>> logger,
        IContentCollectorRepository contentCollectorRepository,
        IContentCollectorRepository collectorRepository,
        IFtpServerService ftpServerService,
        ITBot tBot)
    {
        _logger = logger;
        _contentCollectorRepository = contentCollectorRepository;
        _collectorRepository = collectorRepository;
        _ftpServerService = ftpServerService;
        _tBot = tBot;
    }

    public async Task Handle(ContentProcessedEvent @event)
    {
        Guid contentId = await _contentCollectorRepository.Update(@event.ToContentProcessed());
        if (@event.IsSaved) {
            MemoryStream? contentStream = await _ftpServerService.DownloadFile(@event.Path);

            if (contentStream is null) {
                _logger.LogError("ContentId: {ContentId}. Error while getting content from ftp server.", contentId);
            }
            
            await _tBot.SendVideoAsync(new SendVideoParameters
            {
                Caption = await _collectorRepository.GetVideoCaption(contentId),
                Video = new VideoFile(contentStream!, contentId.ToString()),
                ChatId = await _collectorRepository.GetChatId(contentId) ?? default
            });
        }
    }
}