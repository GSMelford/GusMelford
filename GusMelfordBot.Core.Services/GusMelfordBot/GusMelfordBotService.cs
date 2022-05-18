using System.Net;
using Bot.Api.BotRequests.Interfaces;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Extensions;
using Microsoft.Extensions.Logging;
using Telegram.API.TelegramRequests.GetUpdates;
using Telegram.Bot.Client;
using Telegram.Dto.Response;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.GusMelfordBot;

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

    public Task<List<Update>> GetUpdates(IParameters getUpdatesParameters)
    {
        return _telegramBot.GetUpdates(getUpdatesParameters);
    }

    public async Task<FileResponse?> GetFile(IParameters parameters)
    {
        HttpResponseMessage httpResponseMessage = await _telegramBot.GetFileAsync(parameters);
        return (await httpResponseMessage.Content.ReadAsStringAsync()).ToObject<FileResponse>();
    }
    
    public async Task<byte[]> GetFileBytes(string telegramFilePath)
    {
        HttpResponseMessage httpResponseMessage = await _telegramBot.GetFileBytes(telegramFilePath);
        return await httpResponseMessage.Content.ReadAsByteArrayAsync();
    }
    
    private async Task<HttpResponseMessage> Retry(
        IParameters parameters,
        Func<IParameters, Task<HttpResponseMessage>> telegramMethod)
    {
        try
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
        catch
        {
            //ignored
        }

        return new HttpResponseMessage();
    }
}