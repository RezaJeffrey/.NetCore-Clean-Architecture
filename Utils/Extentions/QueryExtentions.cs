using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils.Expressions;
using Utils.Statics;

namespace Utils.Extentions
{
    public static class QueryExtentions
    {
        public static IQueryable<T> ToPaging<T>(
            this IQueryable<T> query,
            int pageNumber = 1,
            int pageSize = int.MaxValue,
            string orderType = "DESC"
            ) where T : class
        {
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var IdExp = CoreExpression<T>.EntityIdExpression().Compile();

            if (orderType?.ToLower() == "desc")
            {
                query = query.OrderByDescending(IdExp).AsQueryable();
            }
            else
            {
                query = query.OrderBy(IdExp).AsQueryable();
            }

            return query;
        }

        public static async Task<List<T>> ToPagingListAsync<T>(
            this IQueryable<T> query,
            int pageNumber = 1,
            int pageSize = int.MaxValue,
            string orderType = "DESC"
            ) where T : class
        {
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var IdExp = CoreExpression<T>.EntityIdExpression().Compile();

            if (orderType?.ToLower() == "desc")
            {
                query = query.OrderByDescending(IdExp).AsQueryable();
            }
            else
            {
                query = query.OrderBy(IdExp).AsQueryable();
            }

            var result = await Task.Run(() => query.ToList());

            return result;
        }
        public static async Task<(GridData<TDTO> GridData, List<T> Data)> ToPagingGridAsync<T, TDTO>(
            this IQueryable<T> query,
            int pageNumber = 1,
            int pageSize = int.MaxValue,
            string orderType = "DESC"
            ) where T : class where TDTO : class
        {
            var pageCount = (query.Count() + pageSize - 1) / pageSize;

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var IdExp = CoreExpression<T>.EntityIdExpression().Compile();

            if (orderType?.ToLower() == "desc")
            {
                query = query.OrderByDescending(IdExp).AsQueryable();
            }
            else
            {
                query = query.OrderBy(IdExp).AsQueryable();
            }

            var result = await Task.Run(() => query.ToList());

            var paginated = new GridData<TDTO>()
            {
                pageNumber = pageNumber,
                pageSize = pageSize,
                DataCount = result.Count(),
                PageCount = (pageCount <= 0) ? 1 : pageCount
            };


            return (paginated, result);
        }


    }
}
