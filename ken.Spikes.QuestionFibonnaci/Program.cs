using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ken.Spikes.QuestionFibonnaci
{
    internal class Program
    {
        private const int Max = 15;

        private static void Main()
        {
            Console.WriteLine("With Func");
            Func<int, int> fib = null;
            fib = n => n > 1 ? fib(n - 1) + fib(n - 2) : n;
            for (var i = 0; i <= Max; i++)
            {
                Console.WriteLine(fib(i));
            }
            
            Console.WriteLine("With recursion");
            for (var i = 0; i <= Max; i++)
            {
                Console.WriteLine(Fibonacci(i));
            }
        }

        private static int Fibonacci(int x)
        {
            if (x <= 1) return x;
            return Fibonacci(x - 1) + Fibonacci(x - 2);
        }
    }
}
