using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Commands;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Services.Apps;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Commands;

public class CommandService : ICommandService
{
    private readonly IGusMelfordBotService _gusMelfordBotService;
    private readonly ICommandRepository _commandRepository;
    private readonly IContentCollectorService _contentCollectorService;
    private const string COMMAND_BOT_NAME = "@GusMelfordBot";

    public CommandService(
        IGusMelfordBotService gusMelfordBotService, 
        ICommandRepository commandRepository, 
        IContentCollectorService contentCollectorService)
    {
        _gusMelfordBotService = gusMelfordBotService;
        _commandRepository = commandRepository;
        _contentCollectorService = contentCollectorService;
    }
    
    public async Task ProcessCommand(Message message, string applicationType)
    {
        switch (message.Text.Replace(COMMAND_BOT_NAME, ""))
        {
            case Command.RegisterContentCollectorGroup:
                if (await _commandRepository.RegisterContentCollectorGroup(message.Chat))
                {
                    await _gusMelfordBotService.SendMessageAsync(new SendMessageParameters
                    {
                        Text = "Congratulations 🥳, you've registered " +
                               "this conversation as a miscellaneous" +
                               " content gathering conversation. 🥵🤡",
                        ChatId = message.Chat.Id
                    });
                }
                else
                {
                    await _gusMelfordBotService.SendMessageAsync(new SendMessageParameters
                    {
                        Text = "This conversation is already registered with this type. 😎",
                        ChatId = message.Chat.Id
                    });
                }
                break;
            case Command.Refresh:
                switch (applicationType)
                {
                    case App.ContentCollector:
                        await _gusMelfordBotService.SendMessageAsync(new SendMessageParameters
                        {
                            Text = $"Updated {_contentCollectorService.Refresh(message.Chat.Id)} content",
                            ChatId = message.Chat.Id
                        });
                        break;
                }
                break;
        }
    }
}