using System.Net.Http;
using Bot.Api.BotRequests;
using Bot.Api.BotRequests.Interfaces;

namespace Telegram.API.TelegramRequests.DeleteMessage
{
    public class DeleteMessageRequest : Request
    {
        protected override string MethodName => "deleteMessage";
        protected override HttpMethod Method => HttpMethod.Post;
        
        public DeleteMessageRequest(string baseUrl, IParameters parameters = null) 
            : base(baseUrl, parameters)
        {
        }
    }
}