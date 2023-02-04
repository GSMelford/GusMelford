using GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;
using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Domain.Telegram;
using TBot.Client;
using TBot.Client.Api.Telegram.SendMessage;

namespace GusMelfordBot.Api.Services.Telegram.CommandHandlers;

public class SetPasswordCommandHandler : AbstractCommandHandler
{
    private readonly ITBot _tBot;
    private readonly IAuthRepository _authRepository;
    private readonly ILongCommandService _longCommandService;
    
    public SetPasswordCommandHandler(ITBot tBot, IAuthRepository authRepository, ILongCommandService longCommandService)
    {
        _tBot = tBot;
        _authRepository = authRepository;
        _longCommandService = longCommandService;
    }

    public override async Task<TelegramCommand> HandleAsync(TelegramCommand telegramCommand)
    {
        LongCommand? longCommand = _longCommandService.GetLongCommand(telegramCommand.TelegramId);
        if (longCommand is not null && longCommand.Name == Commands.SetPassword)
        {
            await _authRepository.UpdatePasswordAsync(telegramCommand.TelegramId, telegramCommand.Arguments.FirstOrDefault() ?? string.Empty);
            await _tBot.SendMessageAsync(new SendMessageParameters
            {
                Text = "Congratulations! Your password has been updated 🥸",
                ChatId = telegramCommand.ChatId
            });
            _longCommandService.Remove(telegramCommand.TelegramId);

            return telegramCommand;
        }
        
        if (telegramCommand.Name == Commands.SetPassword)
        {
            _longCommandService.TryAdd(telegramCommand.TelegramId, telegramCommand.Name);
            await _tBot.SendMessageAsync(new SendMessageParameters
            {
                Text = "Write your new password in the next message 🥵",
                ChatId = telegramCommand.ChatId
            });

            return telegramCommand;
        }
        
        return await base.HandleAsync(telegramCommand);
    }
}