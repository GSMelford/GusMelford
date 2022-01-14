using System.Net.Http;
using System.Threading.Tasks;

namespace GusMelfordBot.Core.Services.Requests
{
    using Interfaces;
    using Newtonsoft.Json.Linq;
    
    public class RequestService : IRequestService
    {
        private readonly HttpClient _httpClient;

        public RequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage httpRequestMessage)
        {
            return await _httpClient.SendAsync(httpRequestMessage);
        }
    }

    public static class RequestExpression
    {
        public static async Task<JToken> GetJTokenAsync(this HttpResponseMessage httpResponseMessage)
        {
            JToken token = new JObject();
            
            try
            {
                token = JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            catch
            {
                //TODO Log this problem
            }
            
            return token;
        }
    }
}