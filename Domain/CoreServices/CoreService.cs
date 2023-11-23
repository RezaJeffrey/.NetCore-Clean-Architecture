using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoreServices
{
    public class CoreService<T> where T : class
    {
        public DbContext db { get;}
        public DbSet<T> dbTable { get;}

        public CoreService(DbContext dbContext)
        {
            db = dbContext;
            dbTable = db.Set<T>();
            
        }



        /*
        var body = Expression.Or(
              Expression.Equal(PI_DuserId, Expression.Constant(null)),
              Expression.Equal(PI_Ddate, Expression.Constant(null))
            );
        */
        public IQueryable<T> Table()
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");
            var PI_Ddate = Expression.Property(parameter, "Ddate");
            var body = Expression.Equal(PI_Ddate, Expression.Constant(null));

            var nullDdateExpression = Expression.Lambda<Func<T, bool>>(body, parameter);

            return dbTable.Where(nullDdateExpression).AsNoTracking();
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : class
        {
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            var PI_Ddate = Expression.Property(parameter, "Ddate");
            var body = Expression.Equal(PI_Ddate, Expression.Constant(null));

            var nullDdateExpression = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            return db.Set<TEntity>().Where(nullDdateExpression).AsQueryable().AsNoTracking();
        }
        public async Task<T?> FindByIdAsync(long id)
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");
            var PI_ID = Expression.Property(parameter, "Id");
            var body = Expression.Equal(PI_ID, Expression.Constant(id));

            var lambdaIdExpression = Expression.Lambda<Func<T, bool>>(body, parameter);

            return await Table().FirstOrDefaultAsync(lambdaIdExpression);
        }

        

        // Update
        // Add 
        // ToPaging
        // Delete
        // Table<T>
        // CommitAsync
        // BeginTransaction
        // RollbackTransaction
        // FinishTransaction


    }
}
