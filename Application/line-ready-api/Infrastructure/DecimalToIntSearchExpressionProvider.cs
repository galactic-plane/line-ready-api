using System;
using System.Linq.Expressions;

namespace LineReadyApi.Infrastructure
{
    public class DecimalToIntSearchExpressionProvider : ComparableSearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!decimal.TryParse(input, out decimal dec))
                throw new ArgumentException("Invalid search value.");

            byte places = BitConverter.GetBytes(decimal.GetBits(dec)[3])[2];
            if (places < 2) places = 2;
            int justDigits = (int)(dec * ((decimal)Math.Pow(10, places)));

            return Expression.Constant(justDigits);
        }
    }
}