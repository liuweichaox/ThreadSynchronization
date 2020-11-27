using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SpinLockDemo
{
    class Program
    {
        const int N = 100000;
        static Queue<Data> _queue = new Queue<Data>();
        static object _lock = new Object();
        static SpinLock _spinlock = new SpinLock();

        class Data
        {
            public string Name { get; set; }
            public double Number { get; set; }
        }
        static void Main(string[] args)
        {

            //首先使用标准锁进行比较。
            UseLock();
            _queue.Clear();
            UseSpinLock();

            Console.WriteLine("按下一个键");
            Console.ReadKey();
        }

        private static void UpdateWithSpinLock(Data d, int i)
        {
            bool lockTaken = false;
            try
            {
                _spinlock.Enter(ref lockTaken);
                _queue.Enqueue(d);
            }
            finally
            {
                if (lockTaken) _spinlock.Exit(false);
            }
        }

        private static void UseSpinLock()
        {

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                    () => {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithSpinLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    },
                    () => {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithSpinLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    }
                );
            sw.Stop();
            Console.WriteLine("带锁的运行时间: {0}", sw.ElapsedMilliseconds);
        }

        static void UpdateWithLock(Data d, int i)
        {
            lock (_lock)
            {
                _queue.Enqueue(d);
            }
        }

        private static void UseLock()
        {
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                    () => {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    },
                    () => {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    }
                );
            sw.Stop();
            Console.WriteLine("带锁的运行时间: {0}", sw.ElapsedMilliseconds);
        }
    }
}
