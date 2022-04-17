using System.Net;
using Bot.Api.BotRequests.Interfaces;
using GusMelfordBot.Core.Domain.Telegram;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Client;

namespace GusMelfordBot.Core.Services.Telegram;

public class GusMelfordBotService : IGusMelfordBotService
{
    private readonly TelegramBot _telegramBot;
    private const int RETRY_MILLISECONDS = 2000;
    
    public GusMelfordBotService(
        string? token, 
        ILogger<GusMelfordBotService> logger,
        HttpClient httpClient)
    {
        _telegramBot = new TelegramBot(token, new HttpClient(), logger);
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
        return await Retry(parameters, x => _telegramBot.EditMessageAsync(x));
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