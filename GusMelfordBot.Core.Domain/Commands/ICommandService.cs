using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Domain.Commands;

public interface ICommandService
{
    Task ProcessCommand(Message message);
}