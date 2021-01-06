using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LineReadyApi.Infrastructure
{
    public class SortOptionsProcessor<T, TEntity>
    {
        private readonly string[] _orderBy;

        public SortOptionsProcessor(string[] orderBy)
        {
            _orderBy = orderBy;
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            SortTerm[] terms = GetValidTerms().ToArray();

            if (!terms.Any())
            {
                terms = GetTermsFromModel().Where(t => t.Default).ToArray();
            }

            if (!terms.Any()) return query;

            IQueryable<TEntity> modifiedQuery = query;
            bool useThenBy = false;

            foreach (SortTerm term in terms)
            {
                PropertyInfo propertyInfo = ExpressionHelper
                    .GetPropertyInfo<TEntity>(term.EntityName ?? term.Name);
                ParameterExpression obj = ExpressionHelper.Parameter<TEntity>();

                // Build the LINQ expression backwards: query = query.OrderBy(x => x.Property);

                // x => x.Property
                MemberExpression key = ExpressionHelper
                    .GetPropertyExpression(obj, propertyInfo);
                LambdaExpression keySelector = ExpressionHelper
                    .GetLambda(typeof(TEntity), propertyInfo.PropertyType, obj, key);

                // query.OrderBy/ThenBy[Descending](x => x.Property)
                modifiedQuery = ExpressionHelper
                    .CallOrderByOrThenBy(
                        modifiedQuery, useThenBy, term.Descending, propertyInfo.PropertyType, keySelector);

                useThenBy = true;
            }

            return modifiedQuery;
        }

        public IEnumerable<SortTerm> GetAllTerms()
        {
            if (_orderBy == null) yield break;

            foreach (string term in _orderBy)
            {
                if (string.IsNullOrEmpty(term)) continue;

                string[] tokens = term.Split(' ');

                if (tokens.Length == 0)
                {
                    yield return new SortTerm { Name = term };
                    continue;
                }

                bool descending = (tokens.Length > 1) && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

                yield return new SortTerm
                {
                    Name = tokens[0],
                    Descending = descending
                };
            }
        }

        public IEnumerable<SortTerm> GetValidTerms()
        {
            SortTerm[] queryTerms = GetAllTerms().ToArray();
            if (!queryTerms.Any()) yield break;

            IEnumerable<SortTerm> declaredTerms = GetTermsFromModel();

            foreach (SortTerm term in queryTerms)
            {
                SortTerm declaredTerm = declaredTerms
                    .SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));
                if (declaredTerm == null) continue;

                yield return new SortTerm
                {
                    Name = declaredTerm.Name,
                    EntityName = declaredTerm.EntityName,
                    Descending = term.Descending,
                    Default = declaredTerm.Default
                };
            }
        }

        private static IEnumerable<SortTerm> GetTermsFromModel()
        {
            return typeof(T).GetTypeInfo()
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SortableAttribute>().Any())
            .Select(p => new SortTerm
            {
                Name = p.Name,
                EntityName = p.GetCustomAttribute<SortableAttribute>().EntityProperty,
                Default = p.GetCustomAttribute<SortableAttribute>().Default
            });
        }
    }
}