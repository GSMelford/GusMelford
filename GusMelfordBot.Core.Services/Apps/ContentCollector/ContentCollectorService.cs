using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Extensions;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;
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
    private readonly IRequestService _requestService;
    private readonly IFtpServerService _ftpServerService;
    
    public ContentCollectorService(
        ITikTokService tikTokService,
        IContentRepository contentRepository,
        IGusMelfordBotService gusMelfordBotService,
        IRequestService requestService, 
        IFtpServerService ftpServerService)
    {
        _tikTokService = tikTokService;
        _contentRepository = contentRepository;
        _gusMelfordBotService = gusMelfordBotService;
        _requestService = requestService;
        _ftpServerService = ftpServerService;
    }

    public void ProcessMessage(Message message)
    {
        string text = message.Text;

        if (text.Contains(nameof(ContentProvider.TikTok).ToLower()))
        {
            _tikTokService.ProcessMessage(message);
        }
    }

    public async void ProcessCallbackQuery(CallbackQuery callbackQuery)
    {
        string method = callbackQuery.Data.Split(";")[1];

        if (method == "Save")
        {
            var content = await _contentRepository.GetContent(callbackQuery.Data.Split(";")[2].ToGuid());
            
            switch (content?.ContentProvider)
            {
                case nameof(ContentProvider.TikTok):
                    MemoryStream? memoryStream = await _ftpServerService.DownloadFile($"Contents/{content.Name}.mp4");
                    if (memoryStream is null)
                    {
                        TikTokDownloadManager tikTokDownloadManager =
                            new TikTokDownloadManager(_requestService, null);
                        byte[]? contentByte = await tikTokDownloadManager.DownloadTikTokVideo(content);
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
}