using System;
using System.Numerics;
using ken.Core.Math;
using ken.Core.Types;

namespace ken.Spikes.QuestionBillboard
{
    class Program
    {
        static void Main(string[] args)
        {
            FindFirst10DigitPrimeInConsecutiveDigitsOfe();
            Console.ReadKey();
            FindSum49InConsecutiveDigitsOfe();
        }

        /// <summary>
        /// { the first 10-digit prime in consecutive digits of e }.com
        /// </summary>
        public static void FindFirst10DigitPrimeInConsecutiveDigitsOfe()
        {
            var e = BigDecimal.E().ToString(); 
            //var e = StringConstants.Math.e.Replace(".", String.Empty);
            string digits;
            Int64 number;
            var start = 0;
            do
            {
                digits = e.Substring(start, 10);
                number = Int64.Parse(digits);
                Console.WriteLine(digits);
                start++;
            } while (!number.IsPrime());
            Console.WriteLine("Found: {0}.com (at {1})", digits, start);
            // 7427466391.com
            Console.WriteLine(e);
        }

        /// <summary>
        /// f(1) =  7182818284 
        /// f(2) =  8182845904 
        /// f(3) =  8747135266 
        /// f(4) =  7427466391 
        /// f(5) =  ??? 
        /// </summary>
        /// <remarks>
        /// 7+1+8+2+8+1+8+2+8+4 = 49 
        /// 8+1+8+2+8+4+5+9+0+4 = 49 
        /// 8+7+4+7+1+3+5+2+6+6 = 49 
        /// 7+4+2+7+4+6+6+3+9+1 = 49
        /// </remarks>
        private static void FindSum49InConsecutiveDigitsOfe()
        {
            //var e = BigDecimal.E().ToString();
            var e = StringConstants.Math.e.Replace(".", String.Empty);
            string digits;
            BigInteger bigInt;
            var f = 0;
            var start = 0;
            do
            {
                digits = e.Substring(start, 10);
                var sum = 0;
                for (var i = 0; i < 10; i++)
                {
                    sum += digits[i].ToInt32();
                }
                Console.WriteLine("{0} - {1}", digits, sum);
                if (sum == 49) f++;
                start++;
            } while (f < 5);
            Console.WriteLine("Found: f(5) = {0}", digits);
            // 5966290435
        }
    }
}
