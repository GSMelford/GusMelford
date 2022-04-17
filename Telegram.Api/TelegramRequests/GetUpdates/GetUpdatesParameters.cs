using Bot.Api.BotRequests;

namespace Telegram.API.TelegramRequests.GetUpdates
{
    public class GetUpdatesParameters : SerializeParameters
    {
        [ParameterName("offset")]
        public int Offset { get; set; }
        
        [ParameterName("limit")]
        public int Limit { get; set; }
        
        [ParameterName("timeout")]
        public int Timeout { get; set; }
    }
}