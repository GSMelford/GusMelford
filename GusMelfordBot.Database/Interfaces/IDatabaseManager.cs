namespace GusMelfordBot.Database.Interfaces
{
    using DAL;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Context;

    public interface IDatabaseManager
    {
        ApplicationContext Context { get; }
        List<TEntity> Get<TEntity>() where TEntity : DatabaseEntity;
        Task Add<TEntity>(TEntity entity) where TEntity : DatabaseEntity;
        Task<int> Count<TEntity>() where TEntity : DatabaseEntity;
        Task SaveAll();
    }
}