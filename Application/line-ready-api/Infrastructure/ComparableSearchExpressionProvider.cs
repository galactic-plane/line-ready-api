using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LineReadyApi.Infrastructure
{
    public abstract class ComparableSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        private const string GreaterThanEqualToOperator = "gte";
        private const string GreaterThanOperator = "gt";
        private const string LessThanEqualToOperator = "lte";
        private const string LessThanOperator = "lt";

        public override Expression GetComparison(
            MemberExpression left,
            string op,
            ConstantExpression right)
        {
            switch (op.ToLower())
            {
                case GreaterThanOperator: return Expression.GreaterThan(left, right);
                case GreaterThanEqualToOperator: return Expression.GreaterThanOrEqual(left, right);
                case LessThanOperator: return Expression.LessThan(left, right);
                case LessThanEqualToOperator: return Expression.LessThanOrEqual(left, right);
                default: return base.GetComparison(left, op, right);
            }
        }

        public override IEnumerable<string> GetOperators()
        {
            return base.GetOperators()
            .Concat(new[]
            {
                GreaterThanOperator,
                GreaterThanEqualToOperator,
                LessThanOperator,
                LessThanEqualToOperator
            });
        }
    }
}