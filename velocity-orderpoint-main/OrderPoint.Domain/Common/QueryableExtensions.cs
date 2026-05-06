using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Common
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string columnName)
        {
            return ApplyOrdering(query, columnName, "OrderBy");
        }

        public static IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> query, string columnName)
        {
            return ApplyOrdering(query, columnName, "OrderByDescending");
        }

        private static IQueryable<T> ApplyOrdering<T>(IQueryable<T> query, string columnName, string methodName)
        {
            var entityType = typeof(T);
            var property = entityType.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new ArgumentException($"Column '{columnName}' does not exist on type '{entityType.Name}'");

            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.Property(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { entityType, property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
