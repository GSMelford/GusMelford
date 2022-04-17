using System.Net.Http;
using Bot.Api.BotRequests;
using Bot.Api.BotRequests.Interfaces;

namespace Telegram.API.TelegramRequests.EditMessage
{
    public class EditMessageRequest : Request
    {
        protected override string MethodName => "editMessageText";
        protected override HttpMethod Method => HttpMethod.Get;
        
        public EditMessageRequest(string baseUrl, IParameters parameters = null) 
            : base(baseUrl, parameters)
        {
        }
    }
}