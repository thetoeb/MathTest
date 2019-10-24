using System;
using System.Collections.Generic;
using System.Linq;

namespace MathTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("MaxNumber: ");
            var n = Int32.Parse(Console.ReadLine());
            Console.WriteLine("----------------------------");

            for (int i = 1; i <= n; i++)
            {
                var result = GetWinningNumber(i);
                Console.WriteLine(i+" | "+result);
            }

            Console.WriteLine("----------------------------");
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        public static int GetWinningNumber(int count)
        {
            var list = new List<int>(Enumerable.Range(1, count));

            var currentIndex = 0;
            while (list.Count > 1)
            {
                if (currentIndex >= list.Count) currentIndex = currentIndex % list.Count;
                var nextIndex = currentIndex + 1;
                if (nextIndex >= list.Count) nextIndex = nextIndex % list.Count;

                list.RemoveAt(nextIndex);

                currentIndex = nextIndex;
            }

            return list.Single();
        }
    }
}
