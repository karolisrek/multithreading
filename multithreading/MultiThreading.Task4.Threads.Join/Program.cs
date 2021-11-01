/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static Semaphore semaphore = new Semaphore(0, 1);
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            Console.WriteLine("Start part A");

            ACreateTenRecursiveThreads();
            Console.WriteLine("All ten A task threads finished successfully");

            Console.WriteLine("\n------------------------------------------------------\n");

            Console.WriteLine("Start part B");

            BCreateTenRecursiveThreads();
            Console.WriteLine("All ten B task threads finished successfully");

            Console.ReadLine();
        }

        static void ACreateTenRecursiveThreads() => ACreateThread(10);
        static void ACreateThread(int state)
        {
            if (state <= 0)
            {
                return;
            }

            state--;
            Console.WriteLine($"Starting thread with state - {state}");

            Thread thread = new Thread(() => ACreateThread(state));
            thread.Start();
            thread.Join();
        }

        static void BCreateTenRecursiveThreads() => BCreateThread(10);
        static void BCreateThread(int state)
        {
            if (state <= 0)
            {
                semaphore.Release();
                return;
            }

            state--;
            Console.WriteLine($"Starting thread with state - {state}");

            ThreadPool.QueueUserWorkItem(x => BCreateThread(state));
            semaphore.WaitOne();
        }
    }
}
