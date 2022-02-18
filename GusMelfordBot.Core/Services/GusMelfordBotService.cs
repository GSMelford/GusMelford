using System.Net.Http;
using System.Threading;
using RestSharp;

namespace GusMelfordBot.Core.Services
{
    using Bot.Api.BotRequests.Interfaces;
    using Interfaces;
    using Settings;
    using Telegram.Bot.Client;
    
    public class GusMelfordBotService : IGusMelfordBotService
    {
        private readonly TelegramBot _telegramBot;
        private readonly CommonSettings _commonSettings; //TODO Delete after add method to Bot.Api

        public GusMelfordBotService(CommonSettings commonSettings)
        {
            _telegramBot = new TelegramBot(commonSettings.TelegramBotSettings.Token, new HttpClient());
            _commonSettings = commonSettings;
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
        
        public void EditTelegramMessage(string chatId, string text, string messageId)
        {
            //TODO Add method to Bot.Api
            string requestUrl =
                $"https://api.telegram.org/bot{_commonSettings.TelegramBotSettings.Token}/editMessageText";
            
            RestClient restClient = new RestClient();
            RestRequest restRequest = new RestRequest(requestUrl + $"?chat_id={chatId}&text={text}&message_id={messageId}&disable_web_page_preview=true");

            var restResponse = restClient.ExecuteAsync(restRequest).Result;
        }
    }
}