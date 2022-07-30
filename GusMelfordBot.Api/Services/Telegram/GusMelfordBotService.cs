using GusMelfordBot.Domain.Telegram;
using TBot.Client;
using TBot.Client.Api.Telegram.DeleteMessage;
using TBot.Client.Api.Telegram.SendVideo;

namespace GusMelfordBot.Api.Services.Telegram;

public class GusMelfordBotService : IGusMelfordBotService
{
    private readonly ITBot _tBot;

    public GusMelfordBotService(ITBot tBot)
    {
        _tBot = tBot;
    }

    public async Task DeleteMessage(long chatId, int messageId)
    {
        await _tBot.DeleteMessageAsync(new DeleteMessageParameters
        {
            ChatId = chatId,
            MessageId = messageId
        });
    }
}