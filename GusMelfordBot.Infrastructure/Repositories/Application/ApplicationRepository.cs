using GusMelfordBot.Domain.Application;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Infrastructure.Repositories.Application;

public class ApplicationRepository : IApplicationRepository
{
    private readonly IDatabaseContext _databaseContext;
    
    public ApplicationRepository(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<ApplicationService> GetApplicationService(long chatId)
    {
        Feature? application = (await _databaseContext.Set<Group>()
            .Include(x => x.Feature)
            .FirstOrDefaultAsync(x => x.ChatId == chatId))?.Feature;

        return application?.Name switch
        {
            nameof(ApplicationService.ContentCollector) => ApplicationService.ContentCollector,
            _ => ApplicationService.Unknown
        };
    }
}