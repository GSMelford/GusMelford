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
    private readonly IFtpServerService _ftpServerService;
    private readonly ITikTokDownloaderService _tikTokDownloaderService;
    private readonly IContentService _contentService;
    
    public ContentCollectorService(
        ITikTokService tikTokService,
        IContentRepository contentRepository,
        IGusMelfordBotService gusMelfordBotService,
        IFtpServerService ftpServerService,
        ITikTokDownloaderService tikTokDownloaderService, 
        IContentService contentService)
    {
        _tikTokService = tikTokService;
        _contentRepository = contentRepository;
        _gusMelfordBotService = gusMelfordBotService;
        _ftpServerService = ftpServerService;
        _tikTokDownloaderService = tikTokDownloaderService;
        _contentService = contentService;
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
                MemoryStream? memoryStream = await _ftpServerService.DownloadFile($"Contents/{content.Name}.mp4");
                if (memoryStream is null)
                {
                    byte[]? contentByte = await _tikTokDownloaderService.DownloadTikTokVideo(content);
                    if (contentByte is null)
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

                    memoryStream = new MemoryStream(contentByte);
                }
                    
                await _gusMelfordBotService.SendVideoAsync(new SendVideoParameters
                {
                    Video = new VideoFile(memoryStream, content.Name),
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