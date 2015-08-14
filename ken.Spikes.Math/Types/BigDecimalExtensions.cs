using System;
using System.Numerics;

namespace ken.Core.Types
{
    public static class BigDecimalExtensions
    {
        public static BigDecimal Factoral(this BigDecimal number)
        {
            BigDecimal fact = 1;
            BigDecimal i = 1;
            while (i <= number)
            {
                fact = fact*i;
                i++;
            }
            return fact;
        }
    }
}