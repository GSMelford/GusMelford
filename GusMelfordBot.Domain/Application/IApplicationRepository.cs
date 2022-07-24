namespace GusMelfordBot.Domain.Application;

public interface IApplicationRepository
{
    Task<ApplicationService> GetApplicationService(long chatId);
}