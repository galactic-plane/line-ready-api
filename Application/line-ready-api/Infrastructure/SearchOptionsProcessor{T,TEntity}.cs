﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LineReadyApi.Infrastructure
{
    public class SearchOptionsProcessor<T, TEntity>
    {
        private readonly string[] _searchQuery;

        public SearchOptionsProcessor(string[] searchQuery)
        {
            _searchQuery = searchQuery;
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            SearchTerm[] terms = GetValidTerms().ToArray();
            if (!terms.Any()) return query;

            IQueryable<TEntity> modifiedQuery = query;

            foreach (SearchTerm term in terms)
            {
                PropertyInfo propertyInfo = ExpressionHelper
                    .GetPropertyInfo<TEntity>(term.EntityName ?? term.Name);
                ParameterExpression obj = ExpressionHelper.Parameter<TEntity>();

                // Build up the LINQ expression backwards: query = query.Where(x => x.Property == "Value");

                // x.Property
                MemberExpression left = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);
                // "Value"
                ConstantExpression right = term.ExpressionProvider.GetValue(term.Value);

                // x.Property == "Value"
                Expression comparisonExpression = term.ExpressionProvider
                    .GetComparison(left, term.Operator, right);

                // x => x.Property == "Value"
                LambdaExpression lambdaExpression = ExpressionHelper
                    .GetLambda<TEntity, bool>(obj, comparisonExpression);

                // query = query.Where...
                modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambdaExpression);
            }

            return modifiedQuery;
        }

        public IEnumerable<SearchTerm> GetAllTerms()
        {
            if (_searchQuery == null) yield break;

            foreach (string expression in _searchQuery)
            {
                if (string.IsNullOrEmpty(expression)) continue;

                // Each expression looks like: "fieldName op value..."
                string[] tokens = expression.Split(' ');

                if (tokens.Length == 0)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = expression
                    };

                    continue;
                }

                if (tokens.Length < 3)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = tokens[0]
                    };

                    continue;
                }

                yield return new SearchTerm
                {
                    ValidSyntax = true,
                    Name = tokens[0],
                    Operator = tokens[1],
                    Value = string.Join(" ", tokens.Skip(2))
                };
            }
        }

        public IEnumerable<SearchTerm> GetValidTerms()
        {
            SearchTerm[] queryTerms = GetAllTerms()
                .Where(x => x.ValidSyntax)
                .ToArray();

            if (!queryTerms.Any()) yield break;

            IEnumerable<SearchTerm> declaredTerms = GetTermsFromModel();

            foreach (SearchTerm term in queryTerms)
            {
                SearchTerm declaredTerm = declaredTerms
                    .SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));
                if (declaredTerm == null) continue;

                yield return new SearchTerm
                {
                    ValidSyntax = term.ValidSyntax,
                    Name = declaredTerm.Name,
                    EntityName = declaredTerm.EntityName,
                    Operator = term.Operator,
                    Value = term.Value,
                    ExpressionProvider = declaredTerm.ExpressionProvider
                };
            }
        }

        private static IEnumerable<SearchTerm> GetTermsFromModel()
        {
            return typeof(T).GetTypeInfo()
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
            .Select(p =>
            {
                SearchableAttribute attribute = p.GetCustomAttribute<SearchableAttribute>();
                return new SearchTerm
                {
                    Name = p.Name,
                    EntityName = attribute.EntityProperty,
                    ExpressionProvider = attribute.ExpressionProvider
                };
            });
        }
    }
}