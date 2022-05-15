using System.Net.Http;
using Bot.Api.BotRequests;
using Bot.Api.BotRequests.Interfaces;

namespace Telegram.API.TelegramRequests.GetFileBytes
{
    public class GetFileBytesRequest : Request
    {
        protected override string MethodName { get; }
        protected override HttpMethod Method => HttpMethod.Get;
        
        public GetFileBytesRequest(string baseUrl, string telegramFilePath, IParameters parameters = null, IHeaders headers = null) 
            : base(baseUrl, parameters, headers)
        {
            MethodName = telegramFilePath;
        }
    }
}