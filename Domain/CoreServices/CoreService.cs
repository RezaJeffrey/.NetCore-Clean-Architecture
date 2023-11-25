using Domain.ModelMetadata;
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
    public class CoreService<T> where T : BaseModel
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
            return dbTable.Where(Entity => Entity.Ddate == null || Entity.Ddate == 0);
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : BaseModel
        {
            return db.Set<TEntity>().Where(Entity => Entity.Ddate == null || Entity.Ddate == 0);
        }

        public async Task<T?> FindByIdAsync(long id)
        {
            return await Table().FirstOrDefaultAsync(Entity => Entity.Id == id);
        }

        public async Task Create(T T, bool save = true)
        {
            try
            {
                // TODO After implementation of AuthService, Set Current UserName and UserID   
                PropertyInfo? PI_Cdate = T.GetType().GetProperty("Cdate");
                PropertyInfo? PI_CuserId = T.GetType().GetProperty("CuserId");

                if (PI_Cdate == null || PI_CuserId == null)
                {
                    var properties = "";
                    if (PI_CuserId == null) properties += "Cdate, ";
                    if (PI_CuserId == null) properties += "CUserId, ";
                    if (properties.Length > 0) properties = properties.Substring(0, properties.Length - 2);

                    throw new Exception($"Entity doesn't contain properties: {properties}");
                }

                PI_Cdate.SetValue(T, DateTime.Now.Ticks, null);
                PI_CuserId.SetValue(T, (long?)1, null);


                await db.AddAsync(T);
                if (save) await CommitAsync();
            }
            catch (Exception) 
            {
                throw;
            }

        }

        public async Task Delete(long id, bool save = true)
        {
            try
            {
                T? Entity = await FindByIdAsync(id);
                if (Entity == null) throw new Exception("No such Item in DataBase");

                // TODO After implementation of AuthService, Set Current UserName and UserID   
                PropertyInfo? PI_Ddate = Entity.GetType().GetProperty("Ddate");
                PropertyInfo? PI_DuserId = Entity.GetType().GetProperty("DuserId");

                if (PI_Ddate == null || PI_DuserId == null)
                {
                    var properties = "";
                    if (PI_Ddate == null) properties += "Ddate, ";
                    if (PI_DuserId == null) properties += "DuserId, ";
                    if (properties.Length > 0) properties = properties.Substring(0, properties.Length - 2);

                    throw new Exception($"Entity doesn't contain properties: {properties}");
                }

                PI_Ddate.SetValue(Entity, DateTime.Now.Ticks, null);
                PI_DuserId.SetValue(Entity, (long?)1, null);


                dbTable.Update(Entity);
                if (save) await CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task Delete(T InputEntity, bool save = true)  // delete by item object
        {
            try
            {
                T? Entity = await FindByIdAsync(InputEntity.Id);
                if (Entity == null) throw new Exception("No such Item in DataBase");

                /* TODO After implementation of AuthService, Set Current UserName and UserID */  
                PropertyInfo? PI_Ddate = Entity.GetType().GetProperty("Ddate");
                PropertyInfo? PI_DuserId = Entity.GetType().GetProperty("DuserId");

                #region PropertyNullCheck
                if (PI_Ddate == null || PI_DuserId == null)
                {
                    var properties = "";
                    if (PI_Ddate == null) properties += "Ddate, ";
                    if (PI_DuserId == null) properties += "DuserId, ";
                    if (properties.Length > 0) properties = properties.Substring(0, properties.Length - 2);

                    throw new Exception($"Entity doesn't contain properties: {properties}");
                }
                #endregion

                PI_Ddate.SetValue(Entity, DateTime.Now.Ticks, null);
                PI_DuserId.SetValue(Entity, (long?)1, null);


                dbTable.Update(Entity);
                if (save) await CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }

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
         Add DTO convert mapping
         ToPaging

        */
    }
}
