using GusMelfordBot.Core.Domain.Commands;
using GusMelfordBot.Core.Domain.Telegram;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Commands;

public class CommandService : ICommandService
{
    private readonly IGusMelfordBotService _gusMelfordBotService;
    private readonly ICommandRepository _commandRepository;
    private const string COMMAND_BOT_NAME = "@GusMelfordBot";

    public CommandService(
        IGusMelfordBotService gusMelfordBotService, 
        ICommandRepository commandRepository)
    {
        _gusMelfordBotService = gusMelfordBotService;
        _commandRepository = commandRepository;
    }
    
    public async Task ProcessCommand(Message message)
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
        }
    }
}