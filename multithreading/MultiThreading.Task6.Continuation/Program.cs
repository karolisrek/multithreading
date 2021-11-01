/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            StartTaskContinuation();

            Console.ReadLine();
        }

        private static void StartTaskContinuation()
        {
            StartScenarionA();
            StartScenarionB();
            StartScenarionC();
            StartScenarionD();
        }

        private static void StartScenarionA()
        {
            Task taskA;
            Task taskB;

            Console.WriteLine("\nA scenario started\n");

            taskA = Task.Run(() => {
                Console.WriteLine("Parent task finishes with success");
            });

            taskB = taskA.ContinueWith(antecedent => Console.WriteLine($"Child task is being run"));

            Task.WaitAll(taskA, taskB);

            Console.WriteLine();

            try
            {
                taskA = Task.Run(() => {
                    Console.WriteLine("Parent task finishes without success");
                    throw new Exception("This exception is expected!");
                });

                taskB = taskA.ContinueWith(antecedent => Console.WriteLine($"Child task is being run"));

                Task.WaitAll(taskA, taskB);
            } catch(Exception) { }

            Console.WriteLine("\nA scenario finished\n");
        }

        private static void StartScenarionB()
        {
            Task taskA;
            Task taskB;

            Console.WriteLine("\nB scenario started\n");

            try
            {
                taskA = Task.Run(() => {
                    Console.WriteLine("Parent task finishes with success");
                });

                taskB = taskA.ContinueWith((antecedent, _) => Console.WriteLine($"Child task is being run"), null, TaskContinuationOptions.OnlyOnFaulted);

                Task.WaitAll(taskA, taskB);
            }
            catch (Exception) { }

            Console.WriteLine();

            try
            {
                taskA = Task.Run(() => {
                    Console.WriteLine("Parent task finishes without success");
                    throw new Exception("This exception is expected!");
                });

                taskB = taskA.ContinueWith((antecedent, _) => Console.WriteLine($"Child task is being run"), null, TaskContinuationOptions.OnlyOnFaulted);

                Task.WaitAll(taskA, taskB);
            }
            catch (Exception) { }

            Console.WriteLine("\nB scenario finished\n");
        }

        private static void StartScenarionC()
        {
            Task task;

            Console.WriteLine("\nC scenario started\n");

            try
            {
                Action func = () => Console.WriteLine("Parent task finishes with success");

                task = Task.Run(func)
                    .ContinueWith(
                        (antecedent, _) => Console.WriteLine($"Child task is being run"),
                        null,
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously); task.Wait();
            }
            catch (Exception) { }

            Console.WriteLine();

            try
            {
                Action func = () => {
                    Console.WriteLine("Parent task finishes without success");
                    throw new Exception("This exception is expected!");
                };

                task = Task.Run(func)
                    .ContinueWith(
                        (antecedent, _) => Console.WriteLine($"Child task is being run"),
                        null,
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

                task.Wait();
            }
            catch (Exception) { }

            Console.WriteLine("\nC scenario finished\n");
        }

        private static void StartScenarionD()
        {
            Task taskA;
            Task taskB;

            Console.WriteLine("\nD scenario started\n");

            try
            {
                var cancelationSource = new CancellationTokenSource();

                taskA = Task.Run(() => {
                    cancelationSource.Token.ThrowIfCancellationRequested();
                    Console.WriteLine("Parent task finishes with success");
                }, cancelationSource.Token);

                taskB = taskA.ContinueWith(
                    (antecedent, _) => Console.WriteLine($"Child task is being run"),
                    null,
                    TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.HideScheduler);

                Task.WaitAll(taskA, taskB);
            }
            catch (Exception) { }

            Console.WriteLine();

            try
            {
                var cancelationSource = new CancellationTokenSource();
                cancelationSource.Cancel();

                taskA = Task.Run(() => {
                    cancelationSource.Token.ThrowIfCancellationRequested();
                    Console.WriteLine("Parent task finishes with success");
                }, cancelationSource.Token);

                taskB = taskA.ContinueWith(
                    (antecedent, _) => Console.WriteLine($"Child task is being run"),
                    null,
                    TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.HideScheduler);

                Task.WaitAll(taskA, taskB);
            }
            catch (Exception) { }

            Console.WriteLine("\nD scenario finished\n");
        }
    }
}
