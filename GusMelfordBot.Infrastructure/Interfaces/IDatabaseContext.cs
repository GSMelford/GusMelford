using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GusMelfordBot.Infrastructure.Interfaces;

public interface IDatabaseContext
{
    Task InitializeDatabaseAsync(DatabaseSettings databaseSettings);
    void Update<TEntity>(TEntity entity);
    
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, 
        CancellationToken cancellationToken = new ()) where TEntity : class;
    
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
    void RemoveRange(IEnumerable<object> entities);
    
    Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);
    
    void UpdateRange(IEnumerable<object> entities);
    EntityEntry Add(object entity);
    int SaveChanges();
}