using System;
using System.Threading;

namespace MutexDemo
{
    class Program
    {
        // 创建一个新的Mutex。创建的线程不拥有Mutex。
        private static Mutex mut = new Mutex();
        private const int numIterations = 1;
        private const int numThreads = 3;

        static void Main()
        {
            //创建将使用受保护资源的线程。
            for (int i = 0; i < numThreads; i++)
            {
                Thread newThread = new Thread(new ThreadStart(ThreadProc));
                newThread.Name = String.Format("Thread{0}", i + 1);
                newThread.Start();
            }

            //主线程退出，但应用程序继续运行，
            //直到所有前台线程退出。
        }

        private static void ThreadProc()
        {
            for (int i = 0; i < numIterations; i++)
            {
                UseResource();
            }
        }

        //此方法表示必须同步的资源，
        //以便一次只能进入一个线程。
        private static void UseResource()
        {
            // 等到安全的时候再进去。
            Console.WriteLine("{0} 正在请求互斥锁",
                              Thread.CurrentThread.Name);
            mut.WaitOne();

            Console.WriteLine("{0} 已进入保护区",
                              Thread.CurrentThread.Name);

            // 在这里放置访问不可重入资源的代码。

            // 模拟一些工作。
            Thread.Sleep(500);

            Console.WriteLine("{0} 要离开保护区",
                Thread.CurrentThread.Name);

            // Release the Mutex.
            mut.ReleaseMutex();
            Console.WriteLine("{0} 已经释放了互斥锁",
                Thread.CurrentThread.Name);
        }
    }
}
