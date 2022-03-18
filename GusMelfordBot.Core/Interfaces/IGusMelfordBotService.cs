using System.Threading.Tasks;

namespace GusMelfordBot.Core.Interfaces
{
    using System.Net.Http;
    using Bot.Api.BotRequests.Interfaces;
    
    public interface IGusMelfordBotService
    {
        Task<HttpResponseMessage> SendMessageAsync(IParameters parameters);
        Task<HttpResponseMessage> DeleteMessageAsync(IParameters parameters);
        Task<HttpResponseMessage> SendVideoAsync(IParameters parameters);
        Task<HttpResponseMessage> EditMessageAsync(IParameters parameters);
    }
}