using System.Net.Http;
using System.Threading.Tasks;

namespace Bot.Api.BotRequests.Interfaces
{
    public interface IExecutable
    {
        public Task<HttpResponseMessage> Execute(HttpClient httpClient);
    }
}