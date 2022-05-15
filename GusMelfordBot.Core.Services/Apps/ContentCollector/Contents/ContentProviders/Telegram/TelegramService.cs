using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.Telegram;
using GusMelfordBot.Core.Domain.Telegram;
using Telegram.API.TelegramRequests.GetFile;
using Telegram.Dto;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Contents.ContentProviders.Telegram;

public class TelegramService : ITelegramService
{
    private readonly IGusMelfordBotService _gusMelfordBotService;
    
    public TelegramService(IGusMelfordBotService gusMelfordBotService)
    {
        _gusMelfordBotService = gusMelfordBotService;
    }

    public async Task ProcessPhoto(Message message)
    {
        TelegramFile? telegramFile = (await _gusMelfordBotService.GetFile(new GetFileParameters
        {
            FileId = message.Photos.LastOrDefault()?.FileId
        }))?.Result;


        await _gusMelfordBotService.GetFileBytes(telegramFile.FilePath);
    }
}