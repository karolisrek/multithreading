/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static List<int> sharedCollection = new List<int>();
        static bool isGenerationFinished = false;
        static long printedCollectionLenght = 0;
        static ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            StartSharedCollectionGenerationWithPrinting();

            Console.ReadLine();
        }

        private static void StartSharedCollectionGenerationWithPrinting()
        {
            var sharedCollectionGenerator = Task.Run(() => StartGeneratingSharedCollection());
            var sharedCollectionPrinter = Task.Run(() => StartPrintingSharedCollection());

            Task.WaitAll(sharedCollectionGenerator, sharedCollectionPrinter);
        }

        private static void StartGeneratingSharedCollection()
        {
            for (var i = 1; i < 11; i++)
            {
                while (printedCollectionLenght != sharedCollection.Count)
                {
                    Task.Delay(100);
                }

                readerWriterLockSlim.EnterWriteLock();

                sharedCollection.Add(i);

                readerWriterLockSlim.ExitWriteLock();
            }

            isGenerationFinished = true;
        }

        private static void StartPrintingSharedCollection()
        {
            while(!isGenerationFinished)
            {
                readerWriterLockSlim.EnterReadLock();

                if (sharedCollection.Count != printedCollectionLenght)
                {
                    Console.WriteLine($"[{string.Join(",", sharedCollection)}]");
                    printedCollectionLenght++;
                }

                Task.Delay(100);

                readerWriterLockSlim.ExitReadLock();
            }
        }
    }
}
