using System.Text.RegularExpressions;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;
using Microsoft.Extensions.Logging;
using RestSharp;
using Telegram.API.TelegramRequests.SendVideo;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders.TikTok;

public class TikTokService : ITikTokService
{
    private readonly ILogger<TikTokService> _logger;
    private readonly TelegramHelper _telegramHelper;
    private readonly ITikTokRepository _tikTokRepository;
    private readonly TikTokDownloadManager _tikTokDownloadManager;
    private readonly IFtpServerService _ftpServerService;
    private readonly IGusMelfordBotService _gusMelfordBotService;
    
    public TikTokService(
        IGusMelfordBotService gusMelfordBotService, 
        ITikTokRepository tikTokRepository, 
        ILogger<TikTokService> logger,
        IRequestService requestService, 
        IFtpServerService ftpServerService)
    {
        _gusMelfordBotService = gusMelfordBotService;
        _tikTokRepository = tikTokRepository;
        _logger = logger;
        _ftpServerService = ftpServerService;
        _telegramHelper = new TelegramHelper(gusMelfordBotService);
        _tikTokDownloadManager = new TikTokDownloadManager(requestService, null);
    } 
    
    public async Task ProcessMessage(Message message)
    {
        Message? newMessage = await _telegramHelper
            .SendMessageToTelegram(
                TikTokServiceHelper.GetProcessMessage(message),
                message.Chat.Id);
        try
        {
            string? sentLink = GetSendLink(message.Text);
            string refererLink = await GetRefererLink(sentLink);
            var content = await _tikTokRepository.GetContentAsync(refererLink);
            
            if (content is not null)
            {
                _logger.LogWarning("Content exists. {TikTok} {RefererLink}", 
                    nameof(ContentProvider.TikTok), content.RefererLink);
                
                await _telegramHelper.EditMessageFromTelegram(
                    TikTokServiceHelper.GetEditedMessageAboutExist(message), 
                    message.Chat.Id,
                    newMessage?.MessageId ?? 0);
            }
            else
            {
                content = await BuildContentIfNew(message, sentLink, refererLink);
                string videoName = $"{TikTokServiceHelper.GetUserName(content.RefererLink)}" +
                                   $"-{TikTokServiceHelper.GetVideoId(content.RefererLink)}";
                
                content.Name = videoName;
                
                byte[]? array = await _tikTokDownloadManager.DownloadTikTokVideo(content);
                if (array is not null)
                {
                    content.IsSaved = await _ftpServerService.UploadFile(
                    $"Contents/{videoName}.mp4", new MemoryStream(array));
                    await _gusMelfordBotService.SendVideoAsync(new SendVideoParameters
                    {
                        Caption = TikTokServiceHelper.GetEditedMessage(
                            content, 
                            await _tikTokRepository.GetCountAsync(), 
                            content.AccompanyingCommentary),
                        Video = new VideoFile(new MemoryStream(array), videoName),
                        ChatId = message.Chat.Id
                    });
                    await _telegramHelper.DeleteMessageFromTelegram(message.Chat.Id, newMessage?.MessageId ?? 0);
                }
                else
                {
                    await _telegramHelper.EditMessageFromTelegram(
                        TikTokServiceHelper.GetEditedMessage(
                            content, 
                            await _tikTokRepository.GetCountAsync(), 
                            content.AccompanyingCommentary), 
                        message.Chat.Id,
                        newMessage?.MessageId ?? 0);
                }
                
                await _tikTokRepository.SaveContentAsync(content);
                _logger.LogInformation("Content saved successfully. {TikTok} {RefererLink}", 
                    nameof(ContentProvider.TikTok), content.RefererLink);
            }
        }
        catch (global::System.Exception e)
        {
            _logger.LogError("We were unable to save tik tok video content.\n{Message}", e.Message);

            await _telegramHelper.EditMessageFromTelegram(
                TikTokServiceHelper.GetEditedMessageWhetException(message), 
                message.Chat.Id,
                newMessage?.MessageId ?? 0);
        }

        await _telegramHelper.DeleteMessageFromTelegram(message.Chat.Id, message.MessageId);
    }

    private async Task<DAL.Applications.ContentCollector.Content> BuildContentIfNew(
        Message message,
        string? sentLink,
        string refererLink)
    {
        return new DAL.Applications.ContentCollector.Content
        {
            Chat = await _tikTokRepository.GetChatAsync(message.Chat.Id),
            User = await _tikTokRepository.GetUserAsync(message.From.Id),
            AccompanyingCommentary = GetAccompanyingCommentaryIfExist(message),
            SentLink = sentLink,
            RefererLink = refererLink,
            ContentProvider = nameof(ContentProvider.TikTok)
        };
    }

    private static string? GetSendLink(string messageText)
    {
        return messageText.Split(' ', '\n')
            .FirstOrDefault(x => x.Contains(TikTokServiceHelper.TikTok))?.Trim();
    }

    private async Task<string> GetRefererLink(string? sentLink)
    {
        RestClient restClient = new RestClient();
        RestRequest restRequest = new RestRequest(sentLink);
        RestResponse restResponse = await restClient.ExecuteAsync(restRequest);

        Uri? uri = restResponse.ResponseUri;
        if (uri is null)
        {
            return string.Empty;
        }
        
        return uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
    }

    private static string GetAccompanyingCommentaryIfExist(Message message)
    {
        string text = message.Text;
        text = Regex.Replace(text, @"\s+", " ");
        string[] words = text.Trim().Split(" ");

        if (words.Length == 1)
        {
            return string.Empty;
        }
            
        IEnumerable<string> wordsWithoutTikTokLink = words.Where(x => !x.Contains(TikTokServiceHelper.TikTok));
        return $"{message.From.FirstName} {message.From.LastName}: {string.Join(" ", wordsWithoutTikTokLink)}";
    }
}