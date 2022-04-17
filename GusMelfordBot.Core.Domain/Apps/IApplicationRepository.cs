namespace GusMelfordBot.Core.Domain.Apps;

public interface IApplicationRepository
{
    Task<string?> GetApplicationType(long chatId);
    Task RegisterNewUserIfNotExist(global::Telegram.Dto.User userTelegram);
}