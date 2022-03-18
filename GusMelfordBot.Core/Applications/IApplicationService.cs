using System.Threading.Tasks;

namespace GusMelfordBot.Core.Applications
{
    using Telegram.Dto.UpdateModule;
    
    public interface IApplicationService
    {
        Task DefineApplicationFromMessage(Message message);
        void DefineApplicationFromCallbackQuery(CallbackQuery updateCallbackQuery);
    }
}