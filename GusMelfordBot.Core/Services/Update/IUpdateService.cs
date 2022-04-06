using System.Threading.Tasks;

namespace GusMelfordBot.Core.Services.Update;

public interface IUpdateService
{
    Task ProcessUpdate(Telegram.Dto.UpdateModule.Update update);
}