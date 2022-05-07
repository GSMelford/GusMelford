using System.Text.RegularExpressions;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.DAL.Applications.ContentCollector;
using Microsoft.Extensions.Logging;
using Telegram.API.TelegramRequests.SendVideo;
using Telegram.Dto.UpdateModule;
using static GusMelfordBot.Core.Services.Apps.ContentCollector.Contents.ContentProviders.TikTok.TikTokServiceHelper;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Contents.ContentProviders.TikTok;

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

    public async Task ProcessMessageAsync(Message message)
    {
        try
        {
            string? sentTikTokLink = GetSentLink(message.Text);
            if (string.IsNullOrEmpty(sentTikTokLink))
            {
                return;
            }
            
            await PullAndUpdateContentAsync((await PreparingAndSaveContent(message, sentTikTokLink)).Id, message.Chat.Id);
            await _telegramHelper.DeleteMessageFromTelegram(message.Chat.Id, message.MessageId);
            await _telegramHelper.SendMessageToTelegram(
                $" 👍 Content has been saved!\n{sentTikTokLink}",
                message.Chat.Id);
        }
        catch (global::System.Exception e)
        {
            _logger.LogError("Error processing content. " +
                             "ContentProvider: {ContentProvider}. " +
                             "Processing message text: {Text}. " +
                             "Error message: {ErrorMessage}",
                nameof(ContentProvider.TikTok), message.Text, e.Message);
        }
    }

    public async Task PullAndUpdateContentAsync(Guid contentId, long chatId)
    {
        await PullAndUpdateContentHandle(contentId, chatId);
    }

    private async Task PullAndUpdateContentHandle(Guid contentId, long chatId)
    {
        _logger.LogInformation("PullAndUpdateContent started. " +
                               "ContentId: {ContentId} ChatId: {ChatId}", contentId, chatId);
        
        Content? content = _tikTokRepository.FirstOrDefault<Content>(x => x.Id == contentId);
        if (content is null)
        {
            return;
        }

        if (await _tikTokDownloaderService.TryGetAndSaveRefererLink(content))
        {
            await _tikTokRepository.UpdateAndSaveContentAsync(content);
        }
        else
        {
            return;
        }
        
        if (!content.IsSaved)
        {
            content.Name = $"{GetUserName(content.RefererLink)}-{GetVideoId(content.RefererLink)}";
            byte[]? array = await _tikTokDownloaderService.DownloadTikTokVideo(content);
            if (array is not null)
            {
                content.IsSaved = await _ftpServerService.UploadFile(
                    $"Contents/{content.Name}.mp4", new MemoryStream(array));
                await _gusMelfordBotService.SendVideoAsync(new SendVideoParameters
                {
                    Caption = GetEditedMessage(content, content.AccompanyingCommentary),
                    Video = new VideoFile(new MemoryStream(array), content.Name),
                    ChatId = chatId
                });
                
                _logger.LogInformation("Content is fully processed. " +
                                       "Content: {RefererLink} ContentId: {ContentId}", content.RefererLink, content.Id);
            }

            await _tikTokRepository.UpdateAndSaveContentAsync(content);
        }
    }
    
    private async Task<Content> PreparingAndSaveContent(Message message, string? sentTikTokLink)
    {
        Content? content = _tikTokRepository.FirstOrDefault<Content>(x => x.SentLink == sentTikTokLink);
        
        if (content is null)
        {
            content = await BuildContent(message, sentTikTokLink);
            await _tikTokRepository.AddAndSaveContentAsync(content);
            _logger.LogInformation("Content is partially processed. " +
                                   "Content: {SentLink} ContentId: {ContentId}", content.SentLink,  content.Id);
        }

        return content;
    }

    private async Task<Content> BuildContent(Message message, string? sentTikTokLink)
    {
        return new Content
        {
            Chat = await _tikTokRepository.GetChatAsync(message.Chat.Id),
            User = await _tikTokRepository.GetUserAsync(message.From.Id),
            AccompanyingCommentary = message.Text.Replace(sentTikTokLink ?? string.Empty, "").Trim(),
            SentLink = sentTikTokLink,
            ContentProvider = nameof(ContentProvider.TikTok),
            Number = await _tikTokRepository.GetCountAsync() + 1
        };
    }

    private static string? GetSentLink(string messageText)
    {
        string? temp = new Regex(@"https://\w*.tiktok.com/\S{9}/")
            .Matches(messageText)
            .FirstOrDefault()?.Value;

        if (string.IsNullOrEmpty(temp))
        {
            temp = new Regex(@"https://\w*.tiktok.com/\S*")
                .Matches(messageText)
                .FirstOrDefault()?.Value;
        }
        
        return temp;
    }
}