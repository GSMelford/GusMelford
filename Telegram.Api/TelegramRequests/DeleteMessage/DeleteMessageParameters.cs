using Bot.Api.BotRequests;
using Telegram.Dto;

namespace Telegram.API.TelegramRequests.DeleteMessage
{
    public class DeleteMessageParameters : SerializeParameters
    {
        [ParameterName("chat_id", true)] 
        public ChatId ChatId { get; set; }
        
        [ParameterName("message_id", true)] 
        public int MessageId { get; set; }
    }
}