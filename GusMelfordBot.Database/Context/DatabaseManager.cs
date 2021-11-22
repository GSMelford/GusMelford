using GusMelfordBot.Database.Settings;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Database.Context
{
    using DAL;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    
    public class DatabaseManager : IDatabaseContext
    {
        private static ApplicationContext _applicationContext;
        
        public DatabaseManager(DatabaseSettings databaseSettings)
        {
            _applicationContext = new ApplicationContext(databaseSettings);
        }
        
        public List<TEntity> Get<TEntity>() where TEntity : DatabaseEntity
        {
            return _applicationContext.Set<TEntity>().ToList();
        }

        public async Task Add<TEntity>(TEntity entity) where TEntity : DatabaseEntity
        {
            await _applicationContext.AddAsync(entity);
        }

        public Task<int> Count<TEntity>() where TEntity : DatabaseEntity
        {
            return _applicationContext.Set<TEntity>().CountAsync();
        }
        
        public async Task SaveAll()
        {
           await _applicationContext.SaveChangesAsync();
        }
    }
}