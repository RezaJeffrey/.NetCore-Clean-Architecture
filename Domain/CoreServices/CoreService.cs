using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

            var DdateNullBody = Expression.Equal(PI_Ddate, Expression.Constant(null, typeof(long?)));
            var DdateZeroBody = Expression.Equal(PI_Ddate, Expression.Constant( (long?)0, typeof(long?) ));
            var OrBody = Expression.Or(DdateZeroBody, DdateNullBody);

            var nullDdateExpression = Expression.Lambda<Func<T, bool>>(OrBody, parameter);

            return dbTable.Where(nullDdateExpression).AsNoTracking();
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : class
        {
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            var PI_Ddate = Expression.Property(parameter, "Ddate");

            var DdateNullBody = Expression.Equal(PI_Ddate, Expression.Constant(null, typeof(long?)));
            var DdateZeroBody = Expression.Equal(PI_Ddate, Expression.Constant((long?)0, typeof(long?)));
            var OrBody = Expression.Or(DdateZeroBody, DdateNullBody);

            var nullDdateExpression = Expression.Lambda<Func<TEntity, bool>>(OrBody, parameter);

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

        public async Task Add(T T, bool save = true)
        {
            var PI_Cdate = T.GetType().GetProperty("Cdate");
            var PI_CuserId = T.GetType().GetProperty("CuserId");
            //var PI_CuserName = T.GetType().GetProperty("CuserName");

            var Now = DateTime.Now.Ticks;
            var UserId = (long?)1;  // test user id
            var UserName = "TestUserName";  // test user name

            PI_Cdate.SetValue(T, Now, null);
            PI_CuserId.SetValue(T, UserId, null);
            //PI_CuserName.SetValue(T, UserName, null);
            // TODO [AuthService] After Implementing AuthService Set UserName


            await db.AddAsync(T);
            if (save) await CommitAsync();
        }


        public async Task CommitAsync()  // TODO Test Transactions
        {
            await db.SaveChangesAsync();
        }
        
        public async Task BeginTransaction()
        {
            await db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await db.Database.CommitTransactionAsync();
        }

        public async Task RollBackTransaction()
        {
            await db.Database.RollbackTransactionAsync();
        }

        /*
         TODO
         Table All
         Update
         Add 
         Add DTO convert mapping
         ToPaging
         Delete
        */
    }
}
