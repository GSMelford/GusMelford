using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Extensions;
using Newtonsoft.Json.Linq;
using Telegram.API.TelegramRequests.DeleteMessage;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.API.TelegramRequests.SendVideo;
using Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector;

public class ContentCollectorService : IContentCollectorService
{
    private readonly ITikTokService _tikTokService;
    private readonly IContentRepository _contentRepository;
    private readonly IGusMelfordBotService _gusMelfordBotService;
    private readonly ITikTokDownloaderService _tikTokDownloaderService;
    private readonly IContentService _contentService;
    private readonly IDataLakeService _dataLakeService;
    
    public ContentCollectorService(
        ITikTokService tikTokService,
        IContentRepository contentRepository,
        IGusMelfordBotService gusMelfordBotService,
        ITikTokDownloaderService tikTokDownloaderService, 
        IContentService contentService, IDataLakeService dataLakeService)
    {
        _tikTokService = tikTokService;
        _contentRepository = contentRepository;
        _gusMelfordBotService = gusMelfordBotService;
        _tikTokDownloaderService = tikTokDownloaderService;
        _contentService = contentService;
        _dataLakeService = dataLakeService;
    }

    public async Task ProcessMessage(Message message)
    {
        string text = message.Text;

        if (text.Contains(nameof(ContentProvider.TikTok).ToLower()))
        {
            await _tikTokService.ProcessMessageAsync(message);
        }
    }

    public async void ProcessCallbackQuery(CallbackQuery callbackQuery)
    {
        string method = callbackQuery.Data.Split(";")[1];

        switch (method)
        {
            case "Save":
                await Save(callbackQuery);
                break;
        }
    }

    private async Task Save(CallbackQuery callbackQuery)
    {
        var content = await _contentRepository.GetContent(callbackQuery.Data.Split(";")[2].ToGuid());
            
        switch (content?.ContentProvider)
        {
            case nameof(ContentProvider.TikTok):
                byte[]? bytes = await _dataLakeService.Read($"contents/{content.Name}.mp4");
                if (bytes is null)
                {
                    bytes = await _tikTokDownloaderService.DownloadTikTokVideo(content);
                    if (bytes is null)
                    {
                        await _gusMelfordBotService.SendMessageAsync(new SendMessageParameters
                        {
                            Text =
                                "I'm sorry, for some reason I can't send you a video( Keep at least the link:\n" +
                                $"{content.RefererLink ?? "Something is wrong with the link..."}",
                            ChatId = callbackQuery.FromUser.Id
                        });
                        return;
                    }
                }
                
                await _gusMelfordBotService.SendVideoAsync(new SendVideoParameters
                {
                    Video = new VideoFile(new MemoryStream(bytes), content.Name),
                    Caption = content.RefererLink,
                    ChatId = callbackQuery.FromUser.Id
                });
                break;
        }
    }
    
    public async Task<Message?> SendInformationPanelAsync(Guid contentId)
    {
        var content = (await _contentRepository.GetContent(contentId)).ToDomain();

        HttpResponseMessage httpResponseMessage =
            await _gusMelfordBotService.SendMessageAsync(new SendMessageParameters
            {
                Text = "🥵 You are currently watching this\n" +
                       $"{content.RefererLink}",
                ChatId = content.ChatId,
                ReplyMarkup = new InlineKeyboardMarkup
                {
                    Buttons = new[]
                    {
                        new InlineKeyboardButton[]
                        {
                            new()
                            {
                                Text = ContentCollectorButtons.Save,
                                CallbackData = $"ContentCollector;{nameof(ContentCollectorButtons.Save)};{content.Id};"
                            }
                        }
                    }
                }
            });

        JToken token = JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
        return token["result"]?.ToObject<Message>();
    }

    public async void DeleteInformationPanelAsync(Guid chatId, int messageId)
    {
        await _gusMelfordBotService.DeleteMessageAsync(new DeleteMessageParameters
        {
            ChatId = await _contentRepository.GetChatId(chatId),
            MessageId = messageId
        });
    }
    
    
    public Task<int> Refresh(long chatId)
    {
        return _contentService.Refresh(chatId);
    }
}