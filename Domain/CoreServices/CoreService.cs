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

        
        public IQueryable<T> Table() 
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");
            var property = Expression.Property(parameter, "Ddate");
            var body = Expression.Equal(property, Expression.Constant(null));

            var nullDdateExpression = Expression.Lambda<Func<T, bool>>(body, parameter);
            
            return dbTable.Where(nullDdateExpression).AsNoTracking();
        }

    }
}
