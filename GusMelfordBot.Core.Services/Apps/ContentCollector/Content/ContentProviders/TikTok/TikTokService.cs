using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using Microsoft.Extensions.Logging;
using Telegram.API.TelegramRequests.SendVideo;
using Telegram.Dto.UpdateModule;
using RestSharp;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders.TikTok;

public class TikTokService : ITikTokService
{
    private readonly ILogger<TikTokService> _logger;
    private readonly TelegramHelper _telegramHelper;
    private readonly ITikTokRepository _tikTokRepository;
    private readonly ITikTokDownloaderService _tikTokDownloaderService;
    private readonly IFtpServerService _ftpServerService;
    private readonly IGusMelfordBotService _gusMelfordBotService;
    
    public TikTokService(
        IGusMelfordBotService gusMelfordBotService, 
        ITikTokRepository tikTokRepository, 
        ILogger<TikTokService> logger,
        IFtpServerService ftpServerService,
        ITikTokDownloaderService tikTokDownloaderService)
    {
        _gusMelfordBotService = gusMelfordBotService;
        _tikTokRepository = tikTokRepository;
        _logger = logger;
        _ftpServerService = ftpServerService;
        _telegramHelper = new TelegramHelper(gusMelfordBotService);
        _tikTokDownloaderService = tikTokDownloaderService;
    } 
    
    public async Task ProcessMessage(Message message)
    {
        Message? newMessage = new Message();
        string? sentLink = string.Empty;
        try
        {
            newMessage = await _telegramHelper
                .SendMessageToTelegram(TikTokServiceHelper.GetProcessMessage(message), message.Chat.Id);
            
            sentLink = GetSendLink(message.Text);
            string refererLink = await GetRefererLink(sentLink);
            if (string.IsNullOrEmpty(refererLink))
            {
                throw new global::System.Exception("No referrer link");
            }
            
            if (await IsNewContent(refererLink, message, newMessage ?? new Message()))
            {
                var content = await BuildContent(message, sentLink, refererLink);
                await TrySendVideoResult(content, message, newMessage ?? new Message());
                await _tikTokRepository.SaveContentAsync(content);
            
                _logger.LogInformation("Content saved successfully. {TikTok} {RefererLink}", 
                    nameof(ContentProvider.TikTok), content.RefererLink);
            }
        }
        catch (global::System.Exception e)
        {
            _logger.LogError("We were unable to save tik tok video content.\n{Message}", e.Message);
            await EditErrorMessage(message, newMessage ?? new Message(), sentLink ?? string.Empty);
        }

        await _telegramHelper.DeleteMessageFromTelegram(message.Chat.Id, message.MessageId);
    }

    private async Task EditErrorMessage(Message message, Message newMessage, string sentLink)
    {
        await _telegramHelper.DeleteMessageFromTelegram(message.Chat.Id, newMessage.MessageId);
        await _telegramHelper.SendMessageToTelegram(
            TikTokServiceHelper.GetEditedMessageWhetException(message), message.Chat.Id, sentLink);
    }
    
    private async Task TrySendVideoResult(DAL.Applications.ContentCollector.Content content, Message message, Message newMessage)
    {
        string videoName = $"{TikTokServiceHelper.GetUserName(content.RefererLink)}-" +
                           $"{TikTokServiceHelper.GetVideoId(content.RefererLink)}";
            
        content.Name = videoName;
                
        byte[]? array = await _tikTokDownloaderService.DownloadTikTokVideo(content);
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
            await _telegramHelper.DeleteMessageFromTelegram(message.Chat.Id, newMessage.MessageId);
        }
        else
        {
            await _telegramHelper.EditMessageFromTelegram(
                TikTokServiceHelper.GetEditedMessage(
                    content, 
                    await _tikTokRepository.GetCountAsync(), 
                    content.AccompanyingCommentary), 
                message.Chat.Id,
                newMessage.MessageId);
        }
    }
    
    private async Task<bool> IsNewContent(string refererLink, Message message, Message newMessage)
    {
        var content = await _tikTokRepository.GetContentAsync(refererLink);

        if (content is null)
        {
            return true;
        }
        
        _logger.LogWarning("Content exists. {TikTok} {RefererLink}", 
            nameof(ContentProvider.TikTok), content.RefererLink);
                
        await _telegramHelper.EditMessageFromTelegram(
            TikTokServiceHelper.GetEditedMessageAboutExist(message), 
            message.Chat.Id,
            newMessage.MessageId);

        return false;

    }
    
    private async Task<DAL.Applications.ContentCollector.Content> BuildContent(
        Message message,
        string? sentLink,
        string refererLink)
    {
        return new DAL.Applications.ContentCollector.Content
        {
            Chat = await _tikTokRepository.GetChatAsync(message.Chat.Id),
            User = await _tikTokRepository.GetUserAsync(message.From.Id),
            AccompanyingCommentary = TikTokServiceHelper.GetAccompanyingCommentaryIfExist(message),
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
        _logger.LogCritical(restResponse.Content);
        if (uri is null)
        {
            return string.Empty;
        }
        
        return uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
    }
}