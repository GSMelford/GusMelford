using GusMelfordBot.Domain.Telegram.Models;

namespace GusMelfordBot.Domain.Telegram;

public interface IUpdateService
{
    Task ProcessUpdateAsync(UpdateDomain updateDomain);
}