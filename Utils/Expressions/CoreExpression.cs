using System.Linq.Expressions;

namespace Utils.Expressions
{
    public static class CoreExpression<T> where T : class
    {
        public static Expression<Func<T, bool>> EntityIdZeroOrNullExpression()
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");  
            var property = Expression.Property(parameter, "Id");  

            var Equalbody = Expression.Equal(property, Expression.Constant(null)); 
            var ZeroBody = Expression.Equal(property, Expression.Constant((long)0)); 
            var OrBody = Expression.Or(Equalbody, ZeroBody); 

            var expression = Expression.Lambda<Func<T, bool>>(OrBody, parameter);

            return expression;
        }

        public static Expression<Func<T, long>> EntityIdExpression()
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");  
            var property = Expression.Property(parameter, "Id");  
            var body = Expression.Lambda<Func<T, long>>(property, parameter);  

            return body;
        }

        public static Expression<Func<T, bool>> EntityFindByIdExpression(long Id)
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");  
            var property = Expression.Property(parameter, "Id");
            var body = Expression.Equal(property, Expression.Constant(Id));

            var expression = Expression.Lambda<Func<T, bool>>(body, parameter);

            return expression;
        }

        public static long GetEntityIdValue(T Entity)
        {
            if (typeof(T).GetProperty("Id")?.PropertyType != typeof(long))
                throw new InvalidOperationException("The property 'Id' must be of type long.");

            var result = (long?)typeof(T).GetProperty("Id")?.GetValue(Entity);

            if (result == null)
                throw new InvalidOperationException("property value can't be null");

            return (long)result;
        }

    }
}