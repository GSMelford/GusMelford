using System.Text.RegularExpressions;
using GusMelfordBot.Api.Services.Telegram.CommandHandlers;
using GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;
using GusMelfordBot.Domain.Telegram;

namespace GusMelfordBot.Api.Services.Telegram;

public class CommandService : ICommandService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILongCommandService _longCommandService;

    private const string GUS_MELFORD_BOT_NAME = "@GusMelfordBot";
    
    public CommandService(IServiceProvider serviceProvider, ILongCommandService longCommandService)
    {
        _serviceProvider = serviceProvider;
        _longCommandService = longCommandService;
    }
    
    public bool IsCommand(string messageText)
    {
        return messageText.First() == '/';
    }
    
    public bool IsCommandInProgress(long telegramUserId)
    {
        return _longCommandService.GetLongCommand(telegramUserId) is not null;
    }

    public async Task ExecuteAsync(long groupId, long telegramUserId, string input)
    {
        TelegramCommand telegramCommand = BuildTelegramCommand(groupId, telegramUserId, input);
        
        AbstractCommandHandler abstractCommandHandler = 
            ActivatorUtilities.CreateInstance<UserInfoCommandHandler>(_serviceProvider);
        
        abstractCommandHandler
            .SetNext(ActivatorUtilities.CreateInstance<SetPasswordCommandHandler>(_serviceProvider))
            .SetNext(ActivatorUtilities.CreateInstance<AbyssStatisticsCommandHandler>(_serviceProvider));
        
        await abstractCommandHandler.Handle(telegramCommand);
    }
    
    private TelegramCommand BuildTelegramCommand(long groupId, long telegramUserId, string input)
    {
        TelegramCommand telegramCommand = new TelegramCommand
        {
            ChatId = groupId,
            TelegramId = telegramUserId
        };

        var match = Regex.Match(input, "(/\\S*)");
            
        telegramCommand.Name = match.Groups[1].Value.Replace(GUS_MELFORD_BOT_NAME, "");
        telegramCommand.Arguments = match.Groups[2].Value.Split(' ').ToList();
        
        return telegramCommand;
    }
}