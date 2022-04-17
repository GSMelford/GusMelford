using System.Net.Http;
using Bot.Api.BotRequests;
using Bot.Api.BotRequests.Interfaces;

namespace Telegram.API.TelegramRequests.SendMessage
{
    public class SendMessageRequest : Request
    {
        protected override string MethodName => "sendMessage";
        protected override HttpMethod Method => HttpMethod.Post;

        public SendMessageRequest(string baseUrl, IParameters parameters) 
            : base(baseUrl, parameters)
        {
        }
    }
}