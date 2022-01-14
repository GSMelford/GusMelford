using System.Threading.Tasks;

namespace GusMelfordBot.Core.Services.Update
{
    public interface IUpdateService
    {
        void ProcessUpdate(Telegram.Dto.UpdateModule.Update update);
    }
}