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
    private readonly IContentCollectorRepository _collectorRepository;
    private readonly IFtpServerService _ftpServerService;
    private readonly ITBot _tBot;

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
            MemoryStream? contentStream = await _ftpServerService.DownloadFile(@event.Path);
            await _tBot.SendVideoAsync(new SendVideoParameters
            {
                Caption = @event.OriginalLink,
                Video = new VideoFile(contentStream!, @event.Id.ToString()),
                ChatId = await _collectorRepository.GetChatId(@event.Id) ?? default
            });
        }
    }
}