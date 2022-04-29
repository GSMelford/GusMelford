using GusMelfordBot.Database.Interfaces;
using GusMelfordBot.Database.Settings;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Database.Context;

public class DatabaseManager : IDatabaseManager
{
    public ApplicationContext Context { get; }
        
    public DatabaseManager(DatabaseSettings databaseSettings, ILogger<ApplicationContext> logger)
    {
        Context = new ApplicationContext(databaseSettings, logger);
    }
}