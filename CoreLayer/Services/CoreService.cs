using CoreLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Utils.Statics;
using Utils.Expressions;
using Utils.Services;

namespace CoreLayer.Services
{
    public class CoreService<T> : ICoreService<T> where T : class  
    {
        private DbContext db { get;}
        private DbSet<T> dbTable { get;}
        private readonly AuthUtilService AuthUtilService;

        public CoreService(DbContext dbContext, AuthUtilService authUtilService)
        {
            db = dbContext;
            dbTable = db.Set<T>();
            AuthUtilService = authUtilService;
        }


        public DbSet<T> GetDb()
        {
            return dbTable;
        }
        public DbSet<TEntity> GetDb<TEntity>() where TEntity : class
        {
            return db.Set<TEntity>();
        }
        public IQueryable<T> Table()
        {
            return dbTable.AsNoTracking();
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : class
        {
            return db.Set<TEntity>().AsNoTracking();
        }

        public IQueryable<T> TableAll()
        {
            return dbTable.IgnoreQueryFilters();
        }

        public IQueryable<TEntity> TableAll<TEntity>() where TEntity : class
        {
            return db.Set<TEntity>().IgnoreQueryFilters().AsNoTracking();
        }

        public async Task<T?> FindByIdAsync(long id)
        {
            return await Table().FirstOrDefaultAsync(CoreExpression<T>.EntityFindByIdExpression(id));
        }

        public async Task Create(T T, bool save = true)
        {
            try
            { 
                PropertyInfo? PI_Cdate = T.GetType().GetProperty("CreateDate");
                PropertyInfo? PI_CuserId = T.GetType().GetProperty("CreateUserId");

                if (PI_Cdate == null || PI_CuserId == null)
                {
                    var properties = "";
                    if (PI_Cdate == null) properties += "CreateDate, ";
                    if (PI_CuserId == null) properties += "CreateUserId, ";
                    if (properties.Length > 0) properties = properties.Substring(0, properties.Length - 2);

                    throw new Exception($"Entity doesn't contain properties: {properties}");
                }

                PI_Cdate.SetValue(T, DateTime.Now.Ticks, null);
                PI_CuserId.SetValue(
                        T,
                        AuthUtilService.GetUserId(),
                        null
                    );


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
 
                PropertyInfo? PI_Ddate = Entity.GetType().GetProperty("DeleteDate");
                PropertyInfo? PI_DuserId = Entity.GetType().GetProperty("DeleteUserId");

                if (PI_Ddate == null || PI_DuserId == null)
                {
                    var properties = "";
                    if (PI_Ddate == null) properties += "DeleteDate, ";
                    if (PI_DuserId == null) properties += "DeleteUserId, ";
                    if (properties.Length > 0) properties = properties.Substring(0, properties.Length - 2);

                    throw new Exception($"Entity doesn't contain properties: {properties}");
                }

                PI_Ddate.SetValue(Entity, DateTime.Now.Ticks, null);
                PI_DuserId.SetValue(
                        Entity, 
                        AuthUtilService.GetUserId(),
                        null
                    );


                dbTable.Update(Entity);
                if (save) await CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task Delete(T InputEntity, bool save = true) 
        {
            try
            {
                T? Entity = await FindByIdAsync(CoreExpression<T>.GetEntityIdValue(InputEntity));
                if (Entity == null) throw new Exception("No such Item in DataBase");

                PropertyInfo? PI_Ddate = Entity.GetType().GetProperty("DeleteDate");
                PropertyInfo? PI_DuserId = Entity.GetType().GetProperty("DeleteUserId");

                #region PropertyNullCheck
                if (PI_Ddate == null || PI_DuserId == null)
                {
                    var properties = "";
                    if (PI_Ddate == null) properties += "DeleteDate, ";
                    if (PI_DuserId == null) properties += "DeleteUserId, ";
                    if (properties.Length > 0) properties = properties.Substring(0, properties.Length - 2);

                    throw new Exception($"Entity doesn't contain properties: {properties}");
                }
                #endregion

                PI_Ddate.SetValue(Entity, DateTime.Now.Ticks, null);
                PI_DuserId.SetValue(
                        Entity,
                        AuthUtilService.GetUserId(),
                        null
                    ); 


                dbTable.Update(Entity);
                if (save) await CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task Update(T InputEntity, bool save = true)
        {
            try
            {
                T? Entity = await FindByIdAsync(CoreExpression<T>.GetEntityIdValue(InputEntity));
                if (Entity == null) throw new Exception("No such Item in DataBase");

                PropertyInfo? PI_Mdate = InputEntity.GetType().GetProperty("ModifyDate");
                PropertyInfo? PI_MuserId = InputEntity.GetType().GetProperty("ModifyUserId");

                #region PropertyNullCheck
                if (PI_Mdate == null || PI_MuserId == null)
                {
                    var properties = "";
                    if (PI_Mdate == null) properties += "ModifyDate, ";
                    if (PI_MuserId == null) properties += "ModifyUserId, ";
                    if (properties.Length > 0) properties = properties.Substring(0, properties.Length - 2);

                    throw new Exception($"Entity doesn't contain properties: {properties}");
                }
                #endregion

                PI_Mdate.SetValue(InputEntity, DateTime.Now.Ticks, null);
                PI_MuserId.SetValue
                    (
                        InputEntity,
                        AuthUtilService.GetUserId(),
                        null
                    ); 


                dbTable.Update(InputEntity);
                if (save) await CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<GridData<T>> ToPaging(int pageNumber = 1, int pageSize = int.MaxValue, string? orderType = null) 
        {
            var gridData = new GridData<T>();
            var data = await Table().Skip(pageNumber - 1).Take(pageSize).ToListAsync();
            var IdExp = CoreExpression<T>.EntityIdExpression().Compile();

            gridData.Data = (orderType?.ToLower() == "desc") 
                ? data.OrderByDescending(IdExp).ToList() 
                : data.OrderBy(IdExp).ToList();

            gridData.pageSize = pageSize;
            gridData.pageNumber = pageNumber;

            return gridData;
        }

        public async Task CommitAsync()  
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

    }
}
