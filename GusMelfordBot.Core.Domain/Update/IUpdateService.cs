namespace GusMelfordBot.Core.Domain.Update;

public interface IUpdateService
{
    Task<bool> ProcessUpdate(string json);
}