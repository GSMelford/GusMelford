using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bot.Api.BotRequests.Interfaces;
using Telegram.Dto.UpdateModule;

namespace Telegram.Bot.Client
{
    public interface ITelegramBot
    {
        Task<HttpResponseMessage> SendMessageAsync(IParameters parameters);
        Task<HttpResponseMessage> SendVideoAsync(IParameters parameters);
        Task<HttpResponseMessage> DeleteMessageAsync(IParameters parameters);
        Task<HttpResponseMessage> EditMessageAsync(IParameters parameters);
        Task<List<Update>> GetUpdates(IParameters parameters);
    }
}