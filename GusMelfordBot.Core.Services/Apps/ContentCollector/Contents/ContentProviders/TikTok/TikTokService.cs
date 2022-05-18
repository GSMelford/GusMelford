using System.Text.RegularExpressions;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Extensions;
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
    private readonly IGusMelfordBotService _gusMelfordBotService;
    private readonly IDataLakeService _dataLakeService;

    private const string ContentsFolder = "contents";

    public TikTokService(
        IGusMelfordBotService gusMelfordBotService,
        ITikTokRepository tikTokRepository,
        ILogger<TikTokService> logger,
        ITikTokDownloaderService tikTokDownloaderService,
        IDataLakeService dataLakeService)
    {
        _gusMelfordBotService = gusMelfordBotService;
        _tikTokRepository = tikTokRepository;
        _logger = logger;
        _telegramHelper = new TelegramHelper(gusMelfordBotService);
        _tikTokDownloaderService = tikTokDownloaderService;
        _dataLakeService = dataLakeService;
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

            Content content = await PreparingAndSaveContent(message, sentTikTokLink);
            
#pragma warning disable CS4014
            PullAndUpdateContentAsync(content.Id, message.Chat.Id);
#pragma warning restore CS4014
            
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

    public async Task<bool> PullAndUpdateContentAsync(Guid contentId, long chatId, bool retry = false)
    {
        _logger.LogInformation("PullAndUpdateContent started. " +
                               "ContentId: {ContentId} ChatId: {ChatId}", contentId, chatId);

        Content? content = await _tikTokRepository.GetContentAsync(contentId);

        if (content is null)
        {
            return false;
        }

        if (!await _tikTokDownloaderService.TryGetAndSaveRefererLink(content))
        {
            return false;
        }

        if (!retry)
        {
            if (await CheckDuplicate(content))
            {
                return true;
            }
        }

        if (content.IsSaved)
        {
            return true;
        }

        content.Name = $"{GetUserName(content.RefererLink)}-{GetVideoId(content.RefererLink)}";
        byte[]? array = await _tikTokDownloaderService.DownloadTikTokVideo(content);
        
        if (!content.IsValid)
        {
            await _tikTokRepository.UpdateAndSaveContentAsync(content);
            _logger.LogInformation("Content {ContentId} no longer exists", content.Id);
            return true;
        }
        
        if (array is null)
        {
            return false;
        }
        
        content = await SendAndSaveContent(content, array, chatId);
        await _tikTokRepository.UpdateAndSaveContentAsync(content);
        return true;
    }

    private async Task<bool> CheckDuplicate(Content content)
    {
        if (await _tikTokDownloaderService.TryGetAndSaveRefererLink(content))
        {
            Content? foundContent = await _tikTokRepository.GetContentAsync(content.RefererLink);
            if (foundContent is not null)
            {
                await _telegramHelper.SendMessageToTelegram(
                    $"😎 This content №{foundContent.Number} has already been posted by " +
                    $"{foundContent.User.FirstName} {foundContent.User.LastName}\n" +
                    $"{foundContent.RefererLink}", content.Chat.ChatId);
                content.IsValid = false;
                await _tikTokRepository.UpdateAndSaveContentAsync(content);
                return true;
            }
        }
        
        return false;
    }
    
    private async Task<Content> SendAndSaveContent(Content content, byte[] array, long chatId)
    {
        content.IsSaved = true;// await _dataLakeService.Write($"{Path.Combine(ContentsFolder, $"{content.Name}.mp4")}", array);
        if (true)
        {
            Message? newMessage = _telegramHelper.GetMessageResponse(await (await _gusMelfordBotService.SendVideoAsync(
                new SendVideoParameters
                {
                    Caption = GetEditedMessage(content, content.AccompanyingCommentary),
                    Video = new VideoFile(new MemoryStream(array), content.Name),
                    ChatId = chatId
                })).Content.ReadAsStringAsync());

            _logger.LogInformation("Content is fully processed. " +
                                   "Content: {RefererLink} ContentId: {ContentId}", content.RefererLink, content.Id);

            await _telegramHelper.DeleteMessageFromTelegram(content.Chat.ChatId, content.MessageId?.ToInt() ?? 0);
            content.MessageId = newMessage?.MessageId.ToString();
        }
        
        return content;
    }
    
    private async Task<Content> PreparingAndSaveContent(Message message, string? sentTikTokLink)
    {
        Content? content = _tikTokRepository.FirstOrDefault<Content>(x => x.SentLink == sentTikTokLink);
        
        if (content is null)
        {
            content = await BuildContent(message, sentTikTokLink);
            
            Message? newMessage = await _telegramHelper.SendMessageToTelegram(
                $" 👍 Content has been saved!\n{sentTikTokLink}",
                message.Chat.Id);

            content.MessageId = newMessage?.MessageId.ToString();
            
            await _tikTokRepository.AddAndSaveContentAsync(content);
            _logger.LogInformation("Content is partially processed. " +
                                   "Content: {SentLink} ContentId: {ContentId}", content.SentLink,  content.Id);
        }
        
        await _telegramHelper.DeleteMessageFromTelegram(message.Chat.Id, message.MessageId);
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
        return new Regex(@"https://\w*.tiktok.com/\S*")
            .Matches(messageText)
            .FirstOrDefault()?.Value;
    }
}