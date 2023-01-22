namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IAbyssService
{
    Task HandleAsync(AbyssContext abyssContext);
    Task SaveContentAsync(Content content);
}