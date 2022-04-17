using System.Net.Http;
using Bot.Api.BotRequests;
using Bot.Api.BotRequests.Interfaces;

namespace Telegram.API.TelegramRequests.GetUpdates
{
    public class GetUpdatesRequest : Request
    {
        protected override string MethodName => "getUpdates";
        protected override HttpMethod Method => HttpMethod.Post;
        
        public GetUpdatesRequest(string baseUrl, IParameters parameters) 
            : base(baseUrl, parameters)
        {
        }
    }
}