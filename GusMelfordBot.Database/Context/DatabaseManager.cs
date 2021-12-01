namespace GusMelfordBot.Database.Context
{
    using DAL;
    using Settings;
    using Interfaces;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    
    public class DatabaseManager : IDatabaseManager
    {
        public ApplicationContext Context { get; }
        
        public DatabaseManager(DatabaseSettings databaseSettings)
        {
            Context = new ApplicationContext(databaseSettings);
        }
    }
}