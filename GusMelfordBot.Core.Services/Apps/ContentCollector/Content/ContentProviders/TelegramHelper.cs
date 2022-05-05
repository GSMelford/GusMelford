using GusMelfordBot.Core.Domain.Telegram;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.API.TelegramRequests.DeleteMessage;
using Telegram.API.TelegramRequests.EditMessage;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders;

public class TelegramHelper
{
    private readonly IGusMelfordBotService _gusMelfordBotService;
    
    public TelegramHelper(IGusMelfordBotService gusMelfordBotService)
    {
        _gusMelfordBotService = gusMelfordBotService;
    }
    
    public async Task<Message?> SendMessageToTelegram(string text, long chatId, string? messageText = null)
    {
        var parameters = new SendMessageParameters
        {
            Text = text,
            ChatId = chatId,
            DisableNotification = true,
            DisableWebPagePreview = true
        };

        if (messageText is not null)
        {
            parameters.ReplyMarkup = new InlineKeyboardMarkup
            {
                Buttons = new[]
                {
                    new InlineKeyboardButton[]
                    {
                        new()
                        {
                            Text = "To try one more time",
                            CallbackData = $"ContentCollector;Retry;{messageText};"
                        }
                    }
                }
            };
        }
        
        HttpResponseMessage httpResponseMessage = await _gusMelfordBotService.SendMessageAsync(parameters);
        return GetMessageResponse(await httpResponseMessage.Content.ReadAsStringAsync());
    }

    public async Task<Message?> DeleteMessageFromTelegram(long chatId, int messageId)
    {
        HttpResponseMessage httpResponseMessage =
            await _gusMelfordBotService.DeleteMessageAsync(new DeleteMessageParameters
            {
                ChatId = chatId,
                MessageId = messageId
            });

        return GetMessageResponse(await httpResponseMessage.Content.ReadAsStringAsync());
    }

    public async Task<Message?> EditMessageFromTelegram(string newText, long chatId, int messageId)
    {
        HttpResponseMessage httpResponseMessage = await _gusMelfordBotService.EditMessageAsync(
            new EditMessageParameters
            {
                Text = newText,
                ChatId = chatId.ToString(),
                MessageId = messageId,
                DisableWebPagePreview = true
            });
        return GetMessageResponse(await httpResponseMessage.Content.ReadAsStringAsync());
    }

    private Message? GetMessageResponse(string response)
    {
        try
        {
            JToken token = JToken.Parse(response);
            string? messageResponse = token["result"]?.ToString();
            return JsonConvert.DeserializeObject<Message>(messageResponse ?? string.Empty);
        }
        catch
        {
            return new Message();
        }
    }
}