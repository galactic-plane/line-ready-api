using System.Collections.Generic;
using System.Linq.Expressions;

namespace LineReadyApi.Infrastructure
{
    public interface ISearchExpressionProvider
    {
        Expression GetComparison(
            MemberExpression left,
            string op,
            ConstantExpression right);

        IEnumerable<string> GetOperators();

        ConstantExpression GetValue(string input);
    }
}