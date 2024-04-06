using Microsoft.EntityFrameworkCore;
using Utils.Statics;

namespace CoreLayer.Interfaces
{
    public interface ICoreService<T> where T : class 
    {
        public DbSet<T> GetDb();
        public DbSet<TEntity> GetDb<TEntity>() where TEntity : class;
        public IQueryable<T> Table();

        public IQueryable<TEntity> Table<TEntity>() where TEntity : class;

        public IQueryable<T> TableAll();

        public IQueryable<TEntity> TableAll<TEntity>() where TEntity : class;

        public Task<T?> FindByIdAsync(long id);

        public Task Create(T T, bool save = true);

        public Task Delete(long id, bool save = true);

        public Task Delete(T InputEntity, bool save = true);

        public Task Update(T InputEntity, bool save = true);

        public Task<GridData<T>> ToPaging(int pageNumber, int pageSize, string? orderType);

        public Task CommitAsync();

        public Task BeginTransaction();

        public Task CommitTransaction();

        public Task RollBackTransaction();
        
    }
}

