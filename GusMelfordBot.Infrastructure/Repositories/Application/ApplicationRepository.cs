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
        Models.Application? application = (await _databaseContext.Set<TelegramChat>()
            .Include(x => x.Application)
            .FirstOrDefaultAsync(x => x.ChatId == chatId))?.Application;

        return application?.Name switch
        {
            nameof(ApplicationService.ContentCollector) => ApplicationService.ContentCollector,
            _ => ApplicationService.Unknown
        };
    }
}