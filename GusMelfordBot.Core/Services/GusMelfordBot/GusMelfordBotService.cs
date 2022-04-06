using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bot.Api.BotRequests.Interfaces;
using Bot.Api.Collection;
using GusMelfordBot.Core.Interfaces;
using GusMelfordBot.Core.Settings;
using Newtonsoft.Json.Linq;
using Telegram.API.TelegramRequests.DeleteMessage;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.Bot.Client;
using Telegram.Dto;

namespace GusMelfordBot.Core.Services.GusMelfordBot;

public class GusMelfordBotService : IGusMelfordBotService
{
    private readonly TelegramBot _telegramBot;
    private const int RETRY_MILLISECONDS = 1000;
    private readonly CommonSettings _commonSettings; //TODO Delete after add method to Bot.Api

    public GusMelfordBotService(CommonSettings commonSettings)
    {
        _telegramBot = new TelegramBot(commonSettings.TelegramBotSettings.Token, new HttpClient());
        _commonSettings = commonSettings;
    }

    public async Task<HttpResponseMessage> SendMessageAsync(IParameters parameters)
    {
        return await Retry(parameters, x => _telegramBot.SendMessageAsync(x));
    }

    public async Task<HttpResponseMessage> DeleteMessageAsync(IParameters parameters)
    {
        return await Retry(parameters, x => _telegramBot.DeleteMessageAsync(x));
    }

    public async Task<HttpResponseMessage> SendVideoAsync(IParameters parameters)
    {
        return await Retry(parameters, x => _telegramBot.SendVideoAsync(x));
    }
        
    public async Task<HttpResponseMessage> EditMessageAsync(IParameters parameters)
    {
        return await Retry(parameters, TelegramBotEditMessageAsync);
    }

    //TODO Add method to Bot.Api
    private Task<HttpResponseMessage> TelegramBotEditMessageAsync(IParameters parameters)
    {
        EditMessageParameters editMessageParameters = parameters as EditMessageParameters;
            
        string requestUrl =
            $"https://api.telegram.org/bot{_commonSettings.TelegramBotSettings.Token}/editMessageText";
            
        HttpClient httpClient = new HttpClient();
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Get;
        httpRequestMessage.RequestUri = new Uri(requestUrl + editMessageParameters?.GetQuery());

        return httpClient.SendAsync(httpRequestMessage);
    }
        
    private async Task<HttpResponseMessage> Retry(
        IParameters parameters,
        Func<IParameters, Task<HttpResponseMessage>> telegramMethod)
    {
        while (true)
        {
            HttpResponseMessage httpResponseMessage = await telegramMethod(parameters);
            if (!httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.BadRequest)
            {
                await Task.Delay(RETRY_MILLISECONDS);
            }
            else
            {
                return httpResponseMessage;
            }
        }
    }
}

public class EditMessageParameters : IParameters
{
    public string ChatId { get; set; }
    public string Text { get; set; }
    public string MessageId { get; set; }
    public string DisableWebPagePreview { get; set; }

    public string GetQuery()
    {
        return $"?chat_id={ChatId}" +
               $"&text={Text}" +
               $"&message_id={MessageId}" +
               $"&disable_web_page_preview={DisableWebPagePreview}";
    }
        
    public ParameterCollection BuildParameters()
    {
        return new ParameterCollection();
    }
}