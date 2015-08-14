using System;

namespace ken.Core.Types
{
    public static class Int32Extensions
    {
        /// <summary>
        /// Simplistic because it is just Int32
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPrime(this Int32 number)
        {
            if (number < 2) return false;
            if (number % 2 == 0) return (number == 2);
            var root = (int)System.Math.Sqrt(number);
            for (var i = 3; i <= root; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}
