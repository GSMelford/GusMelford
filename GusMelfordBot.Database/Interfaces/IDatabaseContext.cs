namespace GusMelfordBot.Database.Interfaces
{
    using DAL;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IDatabaseContext
    {
        List<TEntity> Get<TEntity>() where TEntity : DatabaseEntity;
        Task Add<TEntity>(TEntity entity) where TEntity : DatabaseEntity;
        Task<int> Count<TEntity>() where TEntity : DatabaseEntity;
        Task SaveAll();
    }
}