namespace GusMelfordBot.Core.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Bot.Api.BotRequests.Interfaces;
    
    public interface IGusMelfordBotService
    {
        HttpResponseMessage SendMessage(IParameters parameters);
        void DeleteMessage(IParameters parameters);
        void SendVideo(IParameters parameters);
    }
}