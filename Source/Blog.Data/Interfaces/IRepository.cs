namespace Blog.Data.Interfaces
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> where TEntity : class, new()
    {
        TEntity Find(params object[] id);

        Task<TEntity> FindAsync(params object[] id);

        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        Task<IQueryable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

        void Remove(TEntity entity);

        void Update(TEntity entity);
    }
}