using Bot.Api.BotRequests;
using Telegram.Dto;
using Telegram.Dto.SendMessage;
using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;

namespace Telegram.API.TelegramRequests.SendMessage
{
    public class SendMessageParameters : SerializeParameters
    {
        [ParameterName("chat_id", true)]
        public ChatId ChatId { get; set; }
        
        [ParameterName("text", true)]
        public string Text { get; set; }
        
        [ParameterName("parseMode")]
        public ParseMode ParseMode { get; set; }
        
        [ParameterName("disable_web_page_preview")]
        public bool DisableWebPagePreview { get; set; }
        
        [ParameterName("disable_notification")]
        public bool DisableNotification { get; set; }
        
        [ParameterName("reply_to_message_id")]
        public int ReplyToMessageId { get; set; }
        
        [ParameterName("reply_markup")]
        public ReplyMarkup ReplyMarkup { get; set; }
    }
}