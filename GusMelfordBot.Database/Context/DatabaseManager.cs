namespace GusMelfordBot.Database.Context
{
    using Settings;
    using Interfaces;
    
    public class DatabaseManager : IDatabaseManager
    {
        public ApplicationContext Context { get; }
        
        public DatabaseManager(DatabaseSettings databaseSettings)
        {
            Context = new ApplicationContext(databaseSettings);
        }
    }
}