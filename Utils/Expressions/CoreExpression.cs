using System.Linq.Expressions;

namespace Utils.Expressions
{
    public static class CoreExpression<T> where T : class
    {
        public static Expression<Func<T, bool>> EntityIdZeroOrNullExpression()
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");  // Entity
            var property = Expression.Property(parameter, "Id");  // Entity.Id

            var Equalbody = Expression.Equal(property, Expression.Constant(null)); // Entity.Id == null
            var ZeroBody = Expression.Equal(property, Expression.Constant((long)0)); // Entity.Id == 0
            var OrBody = Expression.Or(Equalbody, ZeroBody); 

            var expression = Expression.Lambda<Func<T, bool>>(OrBody, parameter);

            return expression;
        }

        public static Expression<Func<T, long>> EntityIdExpression()
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");  // Entity
            var property = Expression.Property(parameter, "Id");  // Entity.Id
            var body = Expression.Lambda<Func<T, long>>(property, parameter);  // Entity => Entity.Id

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

    }
}