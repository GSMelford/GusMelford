using GusMelfordBot.Domain.Application;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Feature = GusMelfordBot.Domain.Application.Feature;

namespace GusMelfordBot.Infrastructure.Repositories.Application;

public class FeatureRepository : IFeatureRepository
{
    private readonly IDatabaseContext _databaseContext;
    
    public FeatureRepository(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Feature> GetFeatureAsync(long chatId)
    {
        Models.Feature? feature = (await _databaseContext.Set<Group>()
            .Include(x => x.Feature)
            .FirstOrDefaultAsync(x => x.ChatId == chatId))?.Feature;

        return feature?.Name switch
        {
            nameof(Feature.Abyss) => Feature.Abyss,
            _ => Feature.Unknown
        };
    }
}