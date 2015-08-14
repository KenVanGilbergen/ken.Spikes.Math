using System;
using System.Diagnostics;
using System.Numerics;

namespace ken.Core.Types
{
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Newton%27s_method
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static BigInteger Sqrt(BigInteger number)
        {
            if (0 == number) return 0;
            var n1 = (number >> 1) + 1;
            var n2 = (n1 + (number/n1)) >> 1;
            while (n2 < n1)
            {
                n1 = n2;
                n2 = (n1 + (number/n1)) >> 1;
            }
            return n1;
        }

        public static bool IsPrime(this BigInteger number)
        {
            if (number < 2) return false;
            if (number % 2 == 0) return (number == 2);

            //var root = Sqrt(number);
            //for (var i = 3; i <= root; i += 2)
            //{
            //    if (number % i == 0)
            //    {
            //        Console.WriteLine("Possible divider: {0} for root: {1}", i, root);
            //        return false;
            //    }
            //}
            //return true;
            var primes = new BigIntegerPrimality();
            return primes.IsPrimeMillerRabin(number);
        }

        public static BigInteger Factoral(this BigInteger x)
        {
            BigInteger fact = 1;
            BigInteger i = 1;
            while (i <= x)
            {
                fact = fact * i;
                i++;
            }
            return fact;
        }
    }
}
