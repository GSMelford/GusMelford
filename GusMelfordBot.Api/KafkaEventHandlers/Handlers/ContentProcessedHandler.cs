using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Extensions.Services.Ftp;
using GusMelfordBot.SimpleKafka.Interfaces;
using TBot.Client;
using TBot.Client.Api.Telegram.SendVideo;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class ContentProcessedHandler : IEventHandler<ContentProcessedEvent>
{
    private readonly IContentCollectorRepository _contentCollectorRepository;
    private readonly ITBot _tBot;
    private readonly IContentCollectorRepository _collectorRepository;
    private readonly IFtpServerService _ftpServerService;

    public ContentProcessedHandler(
        IContentCollectorRepository contentCollectorRepository, 
        IContentCollectorRepository collectorRepository, 
        IFtpServerService ftpServerService, 
        ITBot tBot)
    {
        _contentCollectorRepository = contentCollectorRepository;
        _collectorRepository = collectorRepository;
        _ftpServerService = ftpServerService;
        _tBot = tBot;
    }

    public async Task Handle(ContentProcessedEvent @event)
    {
        await _contentCollectorRepository.Update(@event.ToContentProcessed());

        if (@event.IsSaved)
        {
            string? contentPath = await _collectorRepository.GetContentPath(@event.Id);
            MemoryStream? contentStream = await _ftpServerService.DownloadFile(contentPath!);
            await _tBot.SendVideoAsync(new SendVideoParameters
            {
                Caption = @event.OriginalLink,
                Video = new VideoFile(contentStream!, @event.Id.ToString())
            });
        }
    }
}