using System;
using System.Linq.Expressions;

namespace LineReadyApi.Infrastructure
{
    public class DateTimeSearchExpressionProvider : ComparableSearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!DateTimeOffset.TryParse(input, out DateTimeOffset value))
                throw new ArgumentException("Invalid search value.");

            return Expression.Constant(value);
        }
    }
}