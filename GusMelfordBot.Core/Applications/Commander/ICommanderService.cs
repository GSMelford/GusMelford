using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Applications.Commander
{
    public interface ICommanderService
    {
        void ProcessMessage(Message message);
    }
}