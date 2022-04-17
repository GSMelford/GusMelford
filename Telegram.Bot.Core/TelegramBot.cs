using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bot.Api.BotRequests.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.API.TelegramRequests.DeleteMessage;
using Telegram.API.TelegramRequests.EditMessage;
using Telegram.API.TelegramRequests.GetUpdates;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.API.TelegramRequests.SendVideo;
using Telegram.Dto.Response;
using Telegram.Dto.UpdateModule;

namespace Telegram.Bot.Client
{
    public class TelegramBot : ITelegramBot
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://api.telegram.org/";
        private readonly string _botToken;
        private string BaseUrl => $"{API_URL}bot{_botToken}/";

        private readonly ILogger _logger;
        private readonly UpdateListener _updateListener;
        
        public event UpdateListener.MessageHandler OnMessageUpdate;
        public event UpdateListener.CallbackQueryHandler OnCallbackQueryUpdate;
        public event UpdateListener.EditedMessageHandler OnEditedMessageUpdate;
        
        public TelegramBot(string botToken, HttpClient httpClient, ILogger logger = null)
        {
            _botToken = botToken;
            _httpClient = httpClient;
            _logger = logger;
            
            _updateListener = new UpdateListener();
            _updateListener.OnMessageUpdate += message => OnMessageUpdate?.Invoke(message);
            _updateListener.OnCallbackQueryUpdate += query => OnCallbackQueryUpdate?.Invoke(query);
            _updateListener.OnEditedMessageUpdate += editedMessage => OnEditedMessageUpdate?.Invoke(editedMessage);
        }

        public async Task<HttpResponseMessage> SendMessageAsync(IParameters parameters)
        {
            return await DoRequestAsync(_httpClient, new SendMessageRequest(BaseUrl, parameters));
        }

        public async Task<HttpResponseMessage> SendVideoAsync(IParameters parameters)
        {
            return await DoRequestAsync(_httpClient, new SendVideoRequest(BaseUrl, parameters));
        }

        public async Task<HttpResponseMessage> DeleteMessageAsync(IParameters parameters)
        {
            return await DoRequestAsync(_httpClient, new DeleteMessageRequest(BaseUrl, parameters));
        }
        
        public async Task<HttpResponseMessage> EditMessageAsync(IParameters parameters)
        {
            return await DoRequestAsync(_httpClient, new EditMessageRequest(BaseUrl, parameters));
        }
        
        public async void StartListenUpdateAsync(CancellationToken cancellationToken = default)
        {
            await _updateListener.StartListenUpdateAsync(GetUpdates, cancellationToken);
        }
        
        public async Task<List<Update>> GetUpdates(IParameters parameters)
        {
            HttpResponseMessage httpResponseMessage =
                await DoRequestAsync(_httpClient, new GetUpdatesRequest(BaseUrl, parameters));

            string responseContent = 
                Regex.Unescape(httpResponseMessage.Content.ReadAsStringAsync().Result);
            
            Response response = 
                JsonConvert.DeserializeObject<Response>(responseContent);

            return response?.Result ?? new List<Update>();
        }

        private async Task<HttpResponseMessage> DoRequestAsync(HttpClient httpClient, IExecutable request)
        {
            _logger?.LogInformation($"Sending {request.GetType()} Request");
            HttpResponseMessage response = await request.Execute(httpClient);
            _logger?.LogInformation($"Sent {request.GetType()} Request. StatusCode: {response.StatusCode.ToString()}");

            return response;
        }

        public void StopListenUpdate()
        {
            _updateListener.StopListenUpdate();
        }
    }
}