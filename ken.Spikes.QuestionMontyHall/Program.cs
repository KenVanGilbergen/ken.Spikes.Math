using System;

namespace ken.Spikes.QuestionMontyHall
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Monty_Hall_problem 
    /// </summary>
    class Program
    {
        private const int Iterations = 10000;

        static void Main()
        {
            var r = new Random();

            ulong firstPickCounter = 0;
            ulong secondPickCounter = 0;
            for (var i = 1; i <= Iterations; i++)
            {
                var winningDoor = r.Next(1, 4);
                
                var firstPick = r.Next(1, 4);
                if (winningDoor == firstPick) firstPickCounter++;

                // when I switch for the second door I will only win when my first guess was not winning. 
                if (winningDoor != firstPick) secondPickCounter++;

                Console.WriteLine("win: {0}, guess: {1} ({2:N}%), switch door ({3:N}%)"
                    , winningDoor, firstPick, ((decimal)firstPickCounter / i * 100), ((decimal)secondPickCounter / i * 100));
            }
            Console.WriteLine("Twice as much, but would you change knowing it is a human that decides whether you are allowed to switch?");
        }
    }
}
