using GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;
using GusMelfordBot.Domain.Telegram;
using TBot.Client;
using TBot.Client.Api.Telegram.SendMessage;
using TBot.Telegram.Dto.SendMessage;

namespace GusMelfordBot.Api.Services.Telegram.CommandHandlers;

public class UserInfoCommandHandler : AbstractCommandHandler
{
    private readonly ITBot _tBot;
    private readonly ICommandRepository _commandRepository;
    
    public UserInfoCommandHandler(ITBot tBot, ICommandRepository commandRepository)
    {
        _tBot = tBot;
        _commandRepository = commandRepository;
    }
    
    public override async Task<Command> Handle(Command command)
    {
        if (command.Name == Commands.UserInfo)
        {
            TelegramUserDomain telegramUserDomain = await _commandRepository.GetUser(command.TelegramId);
            await _tBot.SendMessageAsync(new SendMessageParameters
            {
                Text = BuildUserInfoMessage(telegramUserDomain),
                ChatId = command.ChatId,
                ParseMode = ParseMode.Markdown
            });
            
            return command;
        }

        return await base.Handle(command);
    }

    private string BuildUserInfoMessage(TelegramUserDomain telegramUserDomain)
    {
        return  "🥵 Information about you:\n\n" +
               $"📀 Id: {telegramUserDomain.Id}\n" +
               $"💿 TId: {telegramUserDomain.TelegramId}\n" +
               $"🥸 Name: {telegramUserDomain.FirstName} {telegramUserDomain.LastName}\n" +
               $"😐 Username: {telegramUserDomain.Username}\n" +
               $"🔐 Password: {telegramUserDomain.Password}";
    }
}