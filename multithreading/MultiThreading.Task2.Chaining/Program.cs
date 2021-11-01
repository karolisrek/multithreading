/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;

namespace MultiThreading.Task.Chaining
{
    using System.Threading.Tasks;

    class Program
    {
        const int RandomIntCount = 10;
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            StartTaskChain();

            Console.ReadLine();
        }

        static void StartTaskChain()
        {
            var randomIntsTask = Task.Run(() => GenerateRandomInts());
            randomIntsTask.Wait();
            PrintNumbers("Generated numbers", randomIntsTask.Result);
            
            var multipliedRandomIntsTask = Task.Run(() => GetMultipliedNumbers(randomIntsTask.Result));
            multipliedRandomIntsTask.Wait();
            PrintNumbers("Multiplied numbers", multipliedRandomIntsTask.Result);

            var sortedMultipliedRandomIntsTask = Task.Run(() => SortAscending(multipliedRandomIntsTask.Result));
            sortedMultipliedRandomIntsTask.Wait();
            PrintNumbers("Sorted multiplied numbers", sortedMultipliedRandomIntsTask.Result);

            var averageOfSortedMultipliedRandomIntsTask = Task.Run(() => GetAverage(sortedMultipliedRandomIntsTask.Result));
            averageOfSortedMultipliedRandomIntsTask.Wait();
            PrintResults("Average", () => Console.WriteLine(averageOfSortedMultipliedRandomIntsTask.Result));
        }

        static void PrintNumbers(string title, long[] numbers) =>
            PrintResults(title, () => numbers.ToList().ForEach(number => Console.WriteLine(number)));

        static void PrintNumbers(string title, int[] numbers) =>
            PrintResults(title, () => numbers.ToList().ForEach(number => Console.WriteLine(number)));

        static void PrintResults(string title, Action printResults)
        {
            Console.WriteLine($"{title}:");
            printResults();
            Console.WriteLine("--------------------------");
        }

        static int[] GenerateRandomInts()
        {
            var randomInts = new int[RandomIntCount];
            var random = new Random();

            for (var i = 0; i < RandomIntCount; i++)
            {
                randomInts[i] = random.Next(); 
            }

            return randomInts;
        } 

        static long[] GetMultipliedNumbers(int[] numbers)
        {
            var random = new Random();
            long multiplier = random.Next();

            return numbers.Select(x => x * multiplier).ToArray();
        }
        private static long[] SortAscending(long[] numbers) => numbers.OrderBy(x => x).ToArray();
        private static decimal GetAverage(long[] numbers)
        {
            decimal average = 0;
            decimal numbersCount = numbers.Length;

            foreach (var number in numbers)
            {
                average += number / numbersCount;
            }

            return average;
        }
    }
}
