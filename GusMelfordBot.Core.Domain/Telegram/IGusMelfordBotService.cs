using Telegram.Dto.Response;

namespace GusMelfordBot.Core.Domain.Telegram;

using Bot.Api.BotRequests.Interfaces;

public interface IGusMelfordBotService
{
    Task<HttpResponseMessage> SendMessageAsync(IParameters parameters);
    Task<HttpResponseMessage> DeleteMessageAsync(IParameters parameters);
    Task<HttpResponseMessage> SendVideoAsync(IParameters parameters);
    Task<HttpResponseMessage> EditMessageAsync(IParameters parameters);
    Task<List<global::Telegram.Dto.UpdateModule.Update>> GetUpdates(IParameters getUpdatesParameters);
    Task<FileResponse?> GetFile(IParameters parameters);
    Task<byte[]> GetFileBytes(string telegramFilePath);
}