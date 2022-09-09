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

    public override async Task<Command> Handle(Command command)
    {
        if (command.Name == Commands.SetPassword)
        {
            if (!command.IsLongCommandActive) {
                _longCommandService.TryAdd(command.TelegramId, command.Name);
                await _tBot.SendMessageAsync(new SendMessageParameters
                {
                    Text = "Write your new password in the next message 🥵",
                    ChatId = command.ChatId
                });
            }
            else {
                await _authRepository.UpdatePasswordAsync(command.TelegramId, command.Arguments.FirstOrDefault() ?? string.Empty);
                await _tBot.SendMessageAsync(new SendMessageParameters
                {
                    Text = "Congratulations! Your password has been updated 🥸",
                    ChatId = command.ChatId
                });
                _longCommandService.Remove(command.TelegramId);
            }
            
            return command;
        }
        
        return await base.Handle(command);
    }
}