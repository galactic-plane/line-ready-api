using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LineReadyApi.Infrastructure
{
    public static class ExpressionHelper
    {
        private static readonly MethodInfo LambdaMethod = typeof(Expression)
            .GetMethods()
            .First(x => (x.Name == "Lambda") && x.ContainsGenericParameters && (x.GetParameters().Length == 2));

        private static readonly MethodInfo[] QueryableMethods = typeof(Queryable)
            .GetMethods()
            .ToArray();

        public static IQueryable<TEntity> CallOrderByOrThenBy<TEntity>(
            IQueryable<TEntity> modifiedQuery,
            bool useThenBy,
            bool descending,
            Type propertyType,
            LambdaExpression keySelector)
        {
            string methodName = "OrderBy";
            if (useThenBy) methodName = "ThenBy";
            if (descending) methodName += "Descending";

            MethodInfo method = QueryableMethods
                .First(x => (x.Name == methodName) && (x.GetParameters().Length == 2))
                .MakeGenericMethod(new[] { typeof(TEntity), propertyType });

            return (IQueryable<TEntity>)method.Invoke(null, new object[] { modifiedQuery, keySelector });
        }

        public static IQueryable<T> CallWhere<T>(IQueryable<T> query, LambdaExpression predicate)
        {
            MethodInfo whereMethodBuilder = QueryableMethods
                .First(x => (x.Name == "Where") && (x.GetParameters().Length == 2))
                .MakeGenericMethod(new[] { typeof(T) });

            return (IQueryable<T>)whereMethodBuilder
                .Invoke(null, new object[] { query, predicate });
        }

        public static LambdaExpression GetLambda<TSource, TDest>(ParameterExpression obj, Expression arg)
        {
            return GetLambda(typeof(TSource), typeof(TDest), obj, arg);
        }

        public static LambdaExpression GetLambda(Type source, Type dest, ParameterExpression obj, Expression arg)
        {
            MethodInfo lambdaBuilder = GetLambdaFuncBuilder(source, dest);
            return (LambdaExpression)lambdaBuilder.Invoke(null, new object[] { arg, new[] { obj } });
        }

        public static MemberExpression GetPropertyExpression(ParameterExpression obj, PropertyInfo property)
        {
            return Expression.Property(obj, property);
        }

        public static PropertyInfo GetPropertyInfo<T>(string name)
        {
            return typeof(T).GetProperties()
            .Single(p => p.Name == name);
        }

        public static ParameterExpression Parameter<T>()
        {
            return Expression.Parameter(typeof(T));
        }

        private static MethodInfo GetLambdaFuncBuilder(Type source, Type dest)
        {
            Type predicateType = typeof(Func<,>).MakeGenericType(source, dest);
            return LambdaMethod.MakeGenericMethod(predicateType);
        }
    }
}