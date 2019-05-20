using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ThreadPool pool = new ThreadPool())
            {
                List<Action> tasks = new List<Action>();

                for (int i = 0; i < 7; ++i)
                {
                    tasks.Add(new Action(() =>
                    {
                        Console.WriteLine("Task started in {0}", Thread.CurrentThread.Name);
                        Thread.Sleep(200);
                        Console.WriteLine("Task finished in {0}", Thread.CurrentThread.Name);
                    }));
                }

                foreach(var task in tasks)
                {
                    pool.Enqueue(task);
                }

            }

            Console.WriteLine("The work is over. Press any key to finish.");
            Console.ReadKey();
        }
    }
}