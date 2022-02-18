namespace GusMelfordBot.Core.Interfaces
{
    using System.Net.Http;
    using Bot.Api.BotRequests.Interfaces;
    
    public interface IGusMelfordBotService
    {
        HttpResponseMessage SendMessage(IParameters parameters);
        void DeleteMessage(IParameters parameters);
        void SendVideo(IParameters parameters);
        void EditTelegramMessage(string chatId, string text, string messageId);
    }
}