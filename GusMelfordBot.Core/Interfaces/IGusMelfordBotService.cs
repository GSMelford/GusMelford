using System.Net.Http;
using System.Threading.Tasks;

namespace GusMelfordBot.Core.Interfaces
{
    using Bot.Api.BotRequests.Interfaces;
    using Telegram.Bot.Client;
    
    public interface IGusMelfordBotService
    {
        Task SendMessage(IParameters parameters);
        Task DeleteMessage(IParameters parameters);
        void StartListenUpdate();
        bool GetStatus();
        event UpdateListener.MessageHandler OnMessageUpdate;
    }
}