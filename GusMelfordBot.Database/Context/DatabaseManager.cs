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
        
        public List<TEntity> Get<TEntity>() where TEntity : DatabaseEntity
        {
            return Context.Set<TEntity>().ToList();
        }

        public async Task Add<TEntity>(TEntity entity) where TEntity : DatabaseEntity
        {
            await Context.AddAsync(entity);
        }

        public Task<int> Count<TEntity>() where TEntity : DatabaseEntity
        {
            return Context.Set<TEntity>().CountAsync();
        }
        
        public async Task SaveAll()
        {
           await Context.SaveChangesAsync();
        }
    }
}