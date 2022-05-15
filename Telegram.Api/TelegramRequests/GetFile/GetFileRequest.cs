using System.Net.Http;
using Bot.Api.BotRequests;
using Bot.Api.BotRequests.Interfaces;

namespace Telegram.API.TelegramRequests.GetFile
{
    public class GetFileRequest : Request
    {
        protected override string MethodName => "getFile";
        protected override HttpMethod Method => HttpMethod.Get;
        
        public GetFileRequest(string baseUrl, IParameters parameters = null, IHeaders headers = null) 
            : base(baseUrl, parameters, headers)
        {
        }
    }
}