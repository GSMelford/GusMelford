using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Services
{
    using System.Threading.Tasks;
    using Bot.Api.BotRequests.Interfaces;
    using Interfaces;
    using System.Net.Http;
    using Settings;
    using Telegram.Bot.Client;
    
    public class GusMelfordBotService : IGusMelfordBotService
    {
        private readonly TelegramBot _telegramBot;
        private bool _isActive;
        
        public event UpdateListener.MessageHandler OnMessageUpdate;
        public event UpdateListener.CallbackQueryHandler OnCallbackQueryUpdate;
        
        public GusMelfordBotService(TelegramBotSettings telegramBotSettings)
        {
            _telegramBot = new TelegramBot(telegramBotSettings.Token, new HttpClient());
            _telegramBot.OnMessageUpdate += message => OnMessageUpdate?.Invoke(message);
            _telegramBot.OnCallbackQueryUpdate += query => OnCallbackQueryUpdate?.Invoke(query);
        }

        public void StartListenUpdate()
        {
            if (_isActive)
            {
                return;
            }
            
            _telegramBot.StartListenUpdateAsync();
            _isActive = true;
        }
        
        public async Task<HttpResponseMessage> SendMessage(IParameters parameters)
        {
            HttpResponseMessage httpResponseMessage;
            do
            {
                httpResponseMessage = await _telegramBot.SendMessageAsync(parameters);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    await Task.Delay(1000);
                }
                
            } while (!httpResponseMessage.IsSuccessStatusCode);
            
            return httpResponseMessage;
        }
        
        public async Task DeleteMessage(IParameters parameters)
        {
            await _telegramBot.DeleteMessageAsync(parameters);
        }

        public async Task SendVideoAsync(IParameters parameters)
        {
            HttpResponseMessage httpResponseMessage = _telegramBot.SendVideoAsync(parameters).Result;
            JToken token = JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}