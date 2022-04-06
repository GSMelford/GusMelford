using GusMelfordBot.Database.Interfaces;
using GusMelfordBot.Database.Settings;

namespace GusMelfordBot.Database.Context;

public class DatabaseManager : IDatabaseManager
{
    public ApplicationContext Context { get; }
        
    public DatabaseManager(DatabaseSettings databaseSettings)
    {
        Context = new ApplicationContext(databaseSettings);
    }
}