using GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Domain.Telegram;
using TBot.Client;
using TBot.Client.Api.Telegram.SendMessage;

namespace GusMelfordBot.Api.Services.Telegram.CommandHandlers;

public class RegisterAbyssCommandHandler : AbstractCommandHandler
{
    private readonly IAbyssRepository _abyssRepository;
    private readonly ITBot _tBot;

    public RegisterAbyssCommandHandler(IAbyssRepository abyssRepository, ITBot tBot)
    {
        _abyssRepository = abyssRepository;
        _tBot = tBot;
    }

    public override async Task<TelegramCommand> HandleAsync(TelegramCommand telegramCommand)
    {
        if (telegramCommand.Name == Commands.RegisterAbyss)
        {
            if (await _abyssRepository.RegisterGroupAsAbyssAsync(telegramCommand.ChatId))
            {
                await _tBot.SendMessageAsync(new SendMessageParameters
                {
                    ChatId = telegramCommand.ChatId,
                    Text = "Congratulations!🥵 How cool is that!🥰 I'm shocked!\nYour conversation has been registered as Abyss."
                });
            }
            else
            {
                await _tBot.SendMessageAsync(new SendMessageParameters
                {
                    ChatId = telegramCommand.ChatId,
                    Text = "Something went wrong.\nWe can't log your conversation as Abyss.\nBut why, we don’t know, the bot has too little budget to answer this(🤡"
                });
            }

            return telegramCommand;
        }
        
        return await base.HandleAsync(telegramCommand);
    }
}