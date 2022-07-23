using TBot.Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Domain.Telegram;

public interface IUpdateService
{
    Task ProcessUpdate(Update message);
}