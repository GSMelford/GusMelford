using GusMelfordBot.Api.Services.Features.Abyss;
using GusMelfordBot.Domain.Application;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Events;
using Kyoto.Kafka.Interfaces;

namespace GusMelfordBot.Api.Handlers;

public class TelegramMessageReceivedHandler : IKafkaHandler<TelegramMessageReceivedEvent>
{
    private readonly ICommandService _commandService;
    private readonly IFeatureRepository _featureRepository;
    private readonly IAbyssService _abyssService;

    public TelegramMessageReceivedHandler(
        IFeatureRepository featureRepository,
        ICommandService commandService,
        IAbyssService abyssService)
    {
        _featureRepository = featureRepository;
        _commandService = commandService;
        _abyssService = abyssService;
    }

    public async Task HandleAsync(TelegramMessageReceivedEvent @event)
    {
        string? messageText = @event.Message?.Text;
        if (string.IsNullOrEmpty(messageText)) {
            return;
        }

        if (_commandService.IsCommand(messageText) || _commandService.IsCommandInProgress(@event.Message!.From!.Id!.Value))
        {
            await _commandService.ExecuteAsync(
                @event.Message!.Chat!.Id!.Value, 
                @event.Message.From!.Id!.Value, 
                messageText);
            
            return;
        }
        
        switch (await _featureRepository.GetFeatureAsync(@event.Message!.Chat!.Id!.Value))
        {
            case Feature.Abyss:
                await _abyssService.HandleAsync(@event.ToDomain());
                break;
            case Feature.Unknown:
                break;
        }
    }
}