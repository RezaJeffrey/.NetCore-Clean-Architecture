using System.Linq.Expressions;

namespace Utils.Expressions
{
    public class CoreExpression<T> where T : class
    {
        public Expression<Func<T, bool>> EntityIdNullExpression()
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");  // Entity
            var property = Expression.Property(parameter, "Id");  // Entity.Id
            var body = Expression.Equal(property, Expression.Constant(null)); // Entity.Id == null

            var expression = Expression.Lambda<Func<T, bool>>(body, parameter);

            return expression;
        }

        public Expression<Func<T, long>> EntityId()
        {
            var parameter = Expression.Parameter(typeof(T), "Entity");  // Entity
            var property = Expression.Property(parameter, "Id");  // Entity.Id
            var body = Expression.Lambda<Func<T, long>>(property, parameter);  // Entity => Entity.Id

            return body;
        }

    }
}