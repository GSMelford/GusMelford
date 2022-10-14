using GusMelford.TelegramBot.Domain.Telegram;
using GusMelford.TelegramBot.Domain.Update.Interfaces;
using GusMelford.TelegramBot.Services.Update.MessageHandlers.Abstractions;
using GusMelford.TelegramBot.Services.Update.MessageHandlers.Abstractions.Command;

namespace GusMelford.TelegramBot.Services.Update;

public class UpdateService : IUpdateService
{
    private readonly IServiceProvider _serviceProvider;

    public UpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ProcessUpdate(UpdateDomain updateDomain)
    {
        if (updateDomain.Message is not null)
        {
            await HandleMessage(updateDomain.Message);
        }
    }

    private async Task HandleMessage(MessageDomain messageDomain)
    {
        AbstractMessageHandler messageHandler = ActivatorUtilities.CreateInstance<CommandHandler>(_serviceProvider);
        await messageHandler.Handle(messageDomain);
    }
}