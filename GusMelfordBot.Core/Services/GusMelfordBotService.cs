using System.Net.Http;
using System.Threading;

namespace GusMelfordBot.Core.Services
{
    using Bot.Api.BotRequests.Interfaces;
    using Interfaces;
    using Settings;
    using Telegram.Bot.Client;
    
    public class GusMelfordBotService : IGusMelfordBotService
    {
        private readonly TelegramBot _telegramBot;

        public GusMelfordBotService(CommonSettings commonSettings)
        {
            _telegramBot = new TelegramBot(commonSettings.TelegramBotSettings.Token, new HttpClient());
        }
        
        public HttpResponseMessage SendMessage(IParameters parameters)
        {
            HttpResponseMessage httpResponseMessage;
            do
            {
                httpResponseMessage = _telegramBot.SendMessageAsync(parameters).Result;
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    Thread.Sleep(1000);
                }
                
            } while (!httpResponseMessage.IsSuccessStatusCode);
            
            return httpResponseMessage;
        }

        public async void DeleteMessage(IParameters parameters)
        {
            await _telegramBot.DeleteMessageAsync(parameters);
        }

        public async void SendVideo(IParameters parameters)
        {
            await _telegramBot.SendVideoAsync(parameters);
        }
    }
}