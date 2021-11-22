using System.Threading.Tasks;

namespace GusMelfordBot.Core.Services
{
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
        
        public void SendMessage(IParameters parameters)
        {
            _telegramBot.SendMessageAsync(parameters);
        }
        
        public async Task<HttpResponseMessage> DeleteMessage(IParameters parameters)
        {
            return await _telegramBot.DeleteMessageAsync(parameters);
        }
        
        public bool GetStatus()
        {
            return _isActive;
        }
    }
}