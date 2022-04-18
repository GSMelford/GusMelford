using GusMelfordBot.DAL;

namespace GusMelfordBot.Core.Domain.Apps;

public interface IApplicationRepository
{
    Task<List<Chat>> GetChats();
    Task RegisterNewUserIfNotExist(global::Telegram.Dto.User userTelegram);
}