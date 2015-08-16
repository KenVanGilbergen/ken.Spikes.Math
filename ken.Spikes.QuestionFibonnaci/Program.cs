using System;

namespace ken.Spikes.QuestionFibonacci
{
    /// <summary>
    /// In how many ways can you reach the top of a staircase when taking one or to steps at a time?
    /// Eg. 
    /// stair with one step 1 way
    /// stair with two steps 2 ways
    /// stair with three steps 3 ways
    /// stair with four steps 5 ways
    /// ...
    /// </summary>
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
