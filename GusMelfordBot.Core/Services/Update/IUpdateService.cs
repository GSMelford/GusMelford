using System.Threading.Tasks;

namespace GusMelfordBot.Core.Services.Update;

public interface IUpdateService
{
    Task<bool> ProcessUpdate(string json);
}