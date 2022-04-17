using Bot.Api.BotRequests;
using Telegram.Dto;

namespace Telegram.API.TelegramRequests.EditMessage
{
    public class EditMessageParameters : SerializeParameters
    {
        [ParameterName("chat_id", true)] 
        public ChatId ChatId { get; set; }
        
        [ParameterName("text", true)] 
        public string Text { get; set; }
        
        [ParameterName("message_id", true)] 
        public int MessageId { get; set; }
        
        [ParameterName("disable_web_page_preview")]
        public bool DisableWebPagePreview { get; set; }
    }
}