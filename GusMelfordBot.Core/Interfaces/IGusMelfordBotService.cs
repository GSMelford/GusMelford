namespace GusMelfordBot.Core.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Bot.Api.BotRequests.Interfaces;
    using Telegram.Bot.Client;
    
    public interface IGusMelfordBotService
    {
        Task<HttpResponseMessage> SendMessage(IParameters parameters);
        Task DeleteMessage(IParameters parameters);
        Task SendVideoAsync(IParameters parameters);
        void StartListenUpdate();
        event UpdateListener.MessageHandler OnMessageUpdate;
        event UpdateListener.CallbackQueryHandler OnCallbackQueryUpdate;
    }
}