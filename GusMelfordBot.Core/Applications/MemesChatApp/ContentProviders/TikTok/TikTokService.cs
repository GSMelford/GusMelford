using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok;

using Telegram.API.TelegramRequests.DeleteMessage;
using System.Threading.Tasks;
using GusMelfordBot.Core.Services.GusMelfordBot;
using System.Linq;
using GusMelfordBot.Core.Interfaces;
using System;
using System.Net.Http;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
using GusMelfordBot.Database.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.Dto.UpdateModule;
using static TikTokServiceHelper;

public class TikTokService : ITikTokService
{
    private readonly IDatabaseManager _databaseManager;
    private readonly IGusMelfordBotService _gusMelfordBotService;
    private readonly ILogger<TikTokService> _logger;
    
    public TikTokService(
        ILogger<TikTokService> logger,
        IDatabaseManager manager,
        IGusMelfordBotService gusMelfordBotService)
    {
        _databaseManager = manager;
        _gusMelfordBotService = gusMelfordBotService;
        _logger = logger;
    }

    public async Task ProcessMessage(Message message)
    {
        string messageId = await SendMessageToTelegram(GetProcessMessage(message), message.Chat.Id);
        
        try
        {
            TikTokVideoContent tikTokVideoContent = GetContentIfNew(message);

            if (tikTokVideoContent is not null)
            {
                int count = await _databaseManager.Context.Set<TikTokVideoContent>().CountAsync();

                string text = SetAccompanyingCommentaryIfExist(
                    $"{message.From.FirstName} {message.From.LastName}", 
                    message.Text);
                
                await EditMessageFromTelegram(
                    GetEditedMessage(tikTokVideoContent, count, text), 
                    message.Chat.Id,
                    messageId);
               
                tikTokVideoContent.MessageId = int.Parse(messageId);
                tikTokVideoContent.AccompanyingCommentary += text;
                await _databaseManager.Context.AddAsync(tikTokVideoContent);
                await _databaseManager.Context.SaveChangesAsync();
            }
            else
            {
                await EditMessageFromTelegram(
                    GetEditedMessageAboutExist(message), 
                    message.Chat.Id,
                    messageId);
            }
        }
        catch (Exception e)
        {
            _logger?.LogError("We were unable to save tik tok video content.\n{Message}", e.Message);

            if (string.IsNullOrEmpty(messageId))
            {
                return;
            }
            
            await EditMessageFromTelegram(
                GetEditedMessageWhetException(message), 
                message.Chat.Id,
                messageId);
        }

        await DeleteMessageFromTelegram(message.Chat.Id, message.MessageId);
    }
    
    private TikTokVideoContent GetContentIfNew(Message message)
    {
        string sentLink = WithdrawSendLink(message.Text);
        string refererLink = WithdrawRefererLink(sentLink);

        TikTokVideoContent tokVideoContent =
            BuildTikTokVideoContent(
                _databaseManager,
                sentLink,
                refererLink,
                message.From.Id);

        if (tokVideoContent is null)
        {
            return null;
        }

        TikTokVideoContent oldTokVideoContent = _databaseManager.Context
            .Set<TikTokVideoContent>()
            .FirstOrDefault(v => v.RefererLink == tokVideoContent.RefererLink);

        return oldTokVideoContent is not null ? null : tokVideoContent;
    }

    private async Task<string> DeleteMessageFromTelegram(long chatId, int messageId)
    {
        HttpResponseMessage httpResponseMessage =
            await _gusMelfordBotService.DeleteMessageAsync(new DeleteMessageParameters
            {
                ChatId = chatId,
                MessageId = messageId
            });

        return await GusMelfordBotUtils.GetMessageIdFromResponse(httpResponseMessage);
    }
    
    private async Task<string> EditMessageFromTelegram(string newText, long chatId, string messageId)
    {
        HttpResponseMessage httpResponseMessage =
            await _gusMelfordBotService.EditMessageAsync(new EditMessageParameters
            {
                Text = newText,
                ChatId = chatId.ToString(),
                MessageId = messageId,
                DisableWebPagePreview = true.ToString()
            });

        return await GusMelfordBotUtils.GetMessageIdFromResponse(httpResponseMessage);
    }

    private async Task<string> SendMessageToTelegram(string text, long chatId)
    {
        HttpResponseMessage httpResponseMessage =
            await _gusMelfordBotService.SendMessageAsync(new SendMessageParameters
            {
                Text = text,
                ChatId = chatId,
                DisableNotification = true,
                DisableWebPagePreview = true
            });

        return await GusMelfordBotUtils.GetMessageIdFromResponse(httpResponseMessage);
    }
}