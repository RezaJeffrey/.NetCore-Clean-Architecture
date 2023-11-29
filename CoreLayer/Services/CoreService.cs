using AutoMapper.Execution;
using CoreLayer.Interfaces;
using Domain.ModelMetadata;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using Utils.Models;

namespace CoreLayer.Services
{
    public class CoreService<T, TDTO> : ICoreService<T, TDTO> where T : BaseModel where TDTO : class 
    {
        private DbContext db { get;}
        private DbSet<T> dbTable { get;}

        public CoreService(DbContext dbContext)
        {
            db = dbContext;
            dbTable = db.Set<T>();
            
        }


        public IQueryable<T> Table()
        {
            return dbTable.Where(Entity => Entity.DeleteDate == null || Entity.DeleteDate == 0);
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : BaseModel
        {
            return db.Set<TEntity>().Where(Entity => Entity.DeleteDate == null || Entity.DeleteDate == 0);
        }

        public IQueryable<T> TableAll()
        {
            return dbTable;
        }

        public IQueryable<TEntity> TableAll<TEntity>() where TEntity : BaseModel
        {
            return db.Set<TEntity>();
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
                PI_CuserId.SetValue(T, (long?)1, null); // TODO get user ID


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
                PI_DuserId.SetValue(Entity, (long?)1, null); // TODO Get User Id


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
                PI_DuserId.SetValue(Entity, (long?)1, null); // TODO get User Id


                dbTable.Update(Entity);
                if (save) await CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task Update(T InputEntity, bool save = true)
        // TODO Update Not Tested, Test after implementation of Mdate and MuserID in 
        {
            try
            {
                T? Entity = await FindByIdAsync(InputEntity.Id);
                if (Entity == null) throw new Exception("No such Item in DataBase");

                /* TODO After implementation of AuthService, Set Current UserName and UserID */
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
                PI_MuserId.SetValue(InputEntity, (long?)1, null); // TODO get User Id


                dbTable.Update(InputEntity);
                if (save) await CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<GridData<T>> ToPaging(int pageNumber, int pageSize, string? orderType)  // TODO test ToPagin + implement ToPaging by orderfield
        {
            var gridData = new GridData<T>();
            var data = await Table().Skip(pageNumber - 1).Take(pageSize).ToListAsync();

            gridData.Data = (orderType?.ToLower() == "desc") 
                ? data.OrderByDescending(Entity => Entity.Id).ToList() 
                : data.OrderBy(Entity => Entity.Id).ToList();


            return gridData;
        }

        public async Task CommitAsync()  
        {
            await db.SaveChangesAsync();
        }
        
        public async Task BeginTransaction()
        {
            await db.Database.BeginTransactionAsync();
        } // TODO Test Transactions

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

         Add DTO convert mapping
         

        */
    }
}
