namespace GusMelfordBot.Core.Interfaces
{
    using System.Threading.Tasks;
    using Bot.Api.BotRequests.Interfaces;
    using Telegram.Bot.Client;
    
    public interface IGusMelfordBotService
    {
        Task SendMessage(IParameters parameters);
        Task DeleteMessage(IParameters parameters);
        void StartListenUpdate();
        event UpdateListener.MessageHandler OnMessageUpdate;
    }
}