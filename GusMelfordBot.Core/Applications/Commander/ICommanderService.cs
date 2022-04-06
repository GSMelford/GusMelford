using System.Threading.Tasks;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Applications.Commander;

public interface ICommanderService
{
    Task ProcessMessage(Message message);
}