using Telegram.Dto;

namespace GusMelfordBot.Core.Domain.Commands;

public interface ICommandRepository
{
    Task<bool> RegisterContentCollectorGroup(Chat chat);
}