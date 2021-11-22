using System.Net.Http;
using System.Threading.Tasks;

namespace GusMelfordBot.Core.Interfaces
{
    using Bot.Api.BotRequests.Interfaces;
    using Telegram.Bot.Client;
    
    public interface IGusMelfordBotService
    {
        void SendMessage(IParameters parameters);
        Task<HttpResponseMessage> DeleteMessage(IParameters parameters);
        void StartListenUpdate();
        bool GetStatus();
        event UpdateListener.MessageHandler OnMessageUpdate;
    }
}