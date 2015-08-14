using System;

namespace ken.Core.Types
{
    public static class Int64Extensions
    {
        public static bool IsPrime(this Int64 number)
        {
            if (number < 2) return false;
            if (number % 2 == 0) return (number == 2);
            //var root = (int)System.Math.Sqrt(number);
            //for (var i = 3; i <= root; i += 2)
            //{
            //    if (number % i == 0) return false;
            //}
            //return true;
            return MillerRabin((ulong)number);
        }

        // http://mathworld.wolfram.com/LucasPseudoprime.html
        // http://mathworld.wolfram.com/Rabin-MillerStrongPseudoprimeTest.html
        // https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test
        // https://en.wikipedia.org/wiki/AKS_primality_test
        public static bool MillerRabin(ulong n)
        {
            ulong[] ar;
            if (n < 4759123141) ar = new ulong[] { 2, 7, 61 };
            else if (n < 341550071728321) ar = new ulong[] { 2, 3, 5, 7, 11, 13, 17 };
            else ar = new ulong[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 };
            ulong d = n - 1;
            int s = 0;
            while ((d & 1) == 0) { d >>= 1; s++; }
            int i, j;
            for (i = 0; i < ar.Length; i++)
            {
                ulong a = System.Math.Min(n - 2, ar[i]);
                ulong now = MillerRabin_Pow(a, d, n);
                if (now == 1) continue;
                if (now == n - 1) continue;
                for (j = 1; j < s; j++)
                {
                    now = MillerRabin_Mul(now, now, n);
                    if (now == n - 1) break;
                }
                if (j == s) return false;
            }
            return true;
        }

        public static ulong MillerRabin_Mul(ulong a, ulong b, ulong mod)
        {
            int i;
            ulong now = 0;
            for (i = 63; i >= 0; i--) if (((a >> i) & 1) == 1) break;
            for (; i >= 0; i--)
            {
                now <<= 1;
                while (now > mod) now -= mod;
                if (((a >> i) & 1) == 1) now += b;
                while (now > mod) now -= mod;
            }
            return now;
        }

        public static ulong MillerRabin_Pow(ulong a, ulong p, ulong mod)
        {
            if (p == 0) return 1;
            if (p % 2 == 0) return MillerRabin_Pow(MillerRabin_Mul(a, a, mod), p / 2, mod);
            return MillerRabin_Mul(MillerRabin_Pow(a, p - 1, mod), a, mod);
        }
    }
}
