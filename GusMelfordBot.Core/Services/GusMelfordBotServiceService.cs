namespace GusMelfordBot.Core.Services
{
    using System.Threading.Tasks;
    using Bot.Api.BotRequests.Interfaces;
    using Interfaces;
    using System.Net.Http;
    using Settings;
    using Telegram.Bot.Client;
    
    public class GusMelfordBotServiceService : IGusMelfordBotService
    {
        private bool _isActive;
        private readonly TelegramBot _telegramBot;
        
        public event UpdateListener.MessageHandler OnMessageUpdate;
        
        public GusMelfordBotServiceService(TelegramBotSettings telegramBotSettings)
        {
            _telegramBot = new TelegramBot(telegramBotSettings.Token, new HttpClient());
            _telegramBot.OnMessageUpdate += message => OnMessageUpdate?.Invoke(message);
        }

        public void StartListenUpdate()
        {
            _telegramBot.StartListenUpdateAsync();
            _isActive = true;
        }
        
        public async Task SendMessage(IParameters parameters)
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
        }
        
        public async Task DeleteMessage(IParameters parameters)
        {
            HttpResponseMessage httpResponseMessage;
            do
            {
                httpResponseMessage = await _telegramBot.DeleteMessageAsync(parameters);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    await Task.Delay(1000);
                }
                
            } while (!httpResponseMessage.IsSuccessStatusCode);
        }
        
        public bool GetStatus()
        {
            return _isActive;
        }
    }
}