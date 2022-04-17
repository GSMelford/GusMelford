using Bot.Api.BotRequests;
using Telegram.Dto;

namespace Telegram.API.TelegramRequests.SendVideo
{
    public class SendVideoParameters : SerializeParameters
    {
        [ParameterName("chat_id", true)]
        public ChatId ChatId { get; set; }
        
        [ParameterName("video", true)]
        public VideoFile Video { get; set; }
        
        [ParameterName("caption")]
        public string Caption { get; set; }
    }
}