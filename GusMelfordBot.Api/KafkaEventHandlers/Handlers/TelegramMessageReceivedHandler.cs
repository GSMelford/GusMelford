using System.Text.RegularExpressions;
using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.SimpleKafka.Interfaces;
using TBot.Client;
using TBot.Client.Api.Telegram.DeleteMessage;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public class TelegramMessageReceivedHandler : IEventHandler<TelegramMessageReceivedEvent>
{
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IContentCollectorRepository _contentCollectorRepository;
    private readonly ITBot _tBot;
    private readonly HttpClient _httpClient;
    private readonly ICommandService _commandService;
    private readonly ILongCommandService _longCommandService;

    public TelegramMessageReceivedHandler(
        IKafkaProducer<string> kafkaProducer,
        IApplicationRepository applicationRepository,
        IContentCollectorRepository contentCollectorRepository, 
        ITBot tBot, 
        HttpClient httpClient, 
        ICommandService commandService, 
        ILongCommandService longCommandService)
    {
        _kafkaProducer = kafkaProducer;
        _applicationRepository = applicationRepository;
        _contentCollectorRepository = contentCollectorRepository;
        _tBot = tBot;
        _httpClient = httpClient;
        _commandService = commandService;
        _longCommandService = longCommandService;
    }

    public async Task Handle(TelegramMessageReceivedEvent @event)
    {
        long? chatId = @event.Message?.Chat?.Id;

        if (chatId is not null)
        {
            if (await HandleCommand(@event)) {
                return;
            }
            
            switch (await _applicationRepository.GetApplicationService(chatId.Value))
            {
                case ApplicationService.ContentCollector:
                    await HandleContentCollector(@event);
                    break;
                case ApplicationService.Unknown:
                    break;
            }
        }
    }

    private async Task<bool> HandleCommand(TelegramMessageReceivedEvent @event)
    {
        Command command = new Command
        {
            ChatId = @event.Message!.Chat!.Id!.Value,
            TelegramId = @event.Message.From!.Id!.Value
        };

        LongCommand? longCommand = _longCommandService.GetLongCommand(@event.Message!.From!.Id!.Value);
        if (longCommand is null)
        {
            if (@event.Message!.Text![0] == '/')
            {
                var match = Regex.Match(@event.Message!.Text, "(/\\S*)");
                command.Name = match.Groups[1].Value.Replace("@GusMelfordBot", "");
                command.Arguments = match.Groups[2].Value.Split(' ').ToList();
                await _commandService.ExecuteAsync(command);
                return true;
            }
        }
        else
        {
            command.Name = longCommand.Name;
            command.Arguments = @event.Message!.Text!.Split("").ToList();
            command.IsLongCommandActive = true;
            await _commandService.ExecuteAsync(command);
            return true;
        }
        
        return false;
    }
    
    private async Task HandleContentCollector(TelegramMessageReceivedEvent @event)
    {
        string? messageText = @event.Message?.Text;
        if (string.IsNullOrEmpty(messageText)) {
            return;
        }
                   
        Guid contentId = Guid.NewGuid();
        bool isSuccessfullySaved = await _contentCollectorRepository.SaveNew(
            contentId,
            @event.Message?.Chat?.Id,
            @event.Message?.From?.Id,
            messageText,
            @event.Message?.Chat?.Id);

        if (!isSuccessfullySaved) {
            return;
        }
        
        await _kafkaProducer.ProduceAsync(new ContentCollectorMessageEvent
        {
            Id = contentId,
            MessageText = messageText
        });
        
        await _tBot.DeleteMessageAsync(new DeleteMessageParameters
        {
            ChatId = @event.Message!.Chat!.Id!,
            MessageId = @event.Message!.MessageId!.Value
        }, _httpClient);
    }
}