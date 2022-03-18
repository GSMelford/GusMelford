using System.Threading.Tasks;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Applications.MemesChatApp.Interfaces
{
    public interface IMemeChatService
    {
        Task ProcessMessage(Message message);
        void ProcessCallbackQuery(CallbackQuery updateCallbackQuery);
    }
}