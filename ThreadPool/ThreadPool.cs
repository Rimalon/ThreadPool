using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThreadPool
{
    class ThreadPool : IDisposable
    {
        private const int numberOfThreads = 10;
        private volatile bool isDisposed;    
        private List<Thread> threads;
        private Queue<Action> tasks;
        private volatile int numberOfDisposedThreads;

        public ThreadPool()
        {
            isDisposed = false;
            threads = new List<Thread>();
            tasks = new Queue<Action>();
            for (int i = 0; i < numberOfThreads; ++i)
            {
                threads.Add(new Thread(ThreadWork) { Name = "Thread #" + i });
                threads.Last().Start();
            }
            numberOfDisposedThreads = 0;
        }

        public void Enqueue(Action a)
        {
            Monitor.Enter(tasks);
            tasks.Enqueue(a);
            Monitor.PulseAll(tasks);
            Monitor.Exit(tasks);
        }

        public void Dispose()
        {
            while (tasks.Count != 0) ;
            isDisposed = true;
            for (int i = 0; i < numberOfThreads; ++i)
            {
                Enqueue(new Action(() => { }));
            }
            foreach (Thread t in threads)
            {
                t.Join();
            }
        }

        private void ThreadWork()
        {
            while (!isDisposed)
            {
                Monitor.Enter(tasks);
                if (tasks.Count != 0)
                {
                    var task = tasks.Dequeue();
                    Monitor.Exit(tasks);
                    try
                    {
                        task.Invoke();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Incorrect task");
                    }
                }
                else
                {
                    Monitor.Wait(tasks);
                    Monitor.Exit(tasks);
                }
            }
            numberOfDisposedThreads++;
        }

    }
}
