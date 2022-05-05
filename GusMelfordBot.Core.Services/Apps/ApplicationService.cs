using GusMelfordBot.Core.Domain.Apps;
using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Commands;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IContentCollectorService _collectorService;
    private readonly ICommandService _commandService;
    
    public ApplicationService(
        IApplicationRepository applicationRepository,
        IContentCollectorService collectorService,
        ICommandService commandService)
    {
        _applicationRepository = applicationRepository;
        _collectorService = collectorService;
        _commandService = commandService;
    }
    
    public async Task ProcessMessage(Message message)
    {
        await _applicationRepository.RegisterNewUserIfNotExist(message.From);
        string? applicationType = (await _applicationRepository.GetChats())
            .FirstOrDefault(x => x.ChatId == message.Chat.Id)?.ApplicationType;
        
        switch (applicationType)
        {
            case App.ContentCollector:
                _collectorService.ProcessMessage(message);
                break;
        }

        await _commandService.ProcessCommand(message);
    }

    public async void ProcessCallbackQuery(CallbackQuery callbackQuery)
    {
        await _applicationRepository.RegisterNewUserIfNotExist(callbackQuery.FromUser);

        string applicationType = callbackQuery.Data.Split(";")[0];
        switch (applicationType)
        {
            case App.ContentCollector:
                _collectorService.ProcessCallbackQuery(callbackQuery);
                break;
        }
    }
}