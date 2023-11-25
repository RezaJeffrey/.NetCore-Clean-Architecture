using CoreLayer.Utils;
using Domain.ModelMetadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Interfaces
{
    public interface ICoreService<T> where T : BaseModel
    {
        public IQueryable<T> Table();

        public IQueryable<TEntity> Table<TEntity>() where TEntity : BaseModel;

        public IQueryable<T> TableAll();

        public IQueryable<TEntity> TableAll<TEntity>() where TEntity : BaseModel;

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

