using System;
using System.Threading;

namespace InterlockedDemo
{
    class Program
    {
        // 0为false，1为true。
        private static int usingResource = 0;

        private const int numThreadIterations = 5;
        private const int numThreads = 10;

        static void Main()
        {
            Thread myThread;
            Random rnd = new Random();

            for (int i = 0; i < numThreads; i++)
            {
                myThread = new Thread(new ThreadStart(MyThreadProc));
                myThread.Name = String.Format("Thread{0}", i + 1);

                //等待下一个线程开始之前的随机时间。
                Thread.Sleep(rnd.Next(0, 1000));
                myThread.Start();
            }
        }

        private static void MyThreadProc()
        {
            for (int i = 0; i < numThreadIterations; i++)
            {
                UseResource();

                //等待1秒钟，然后再尝试下一次。
                Thread.Sleep(1000);
            }
        }

        // 一个拒绝再进入的简单方法；
        static bool UseResource()
        {
            //0表示该方法未被使用。
            if (0 == Interlocked.Exchange(ref usingResource, 1))
            {
                Console.WriteLine("{0} acquired the lock", Thread.CurrentThread.Name);

                //访问非线程安全资源的代码将在这里。

                //模拟一些工作
                Thread.Sleep(500);

                Console.WriteLine("{0} exiting lock", Thread.CurrentThread.Name);

                //释放锁
                Interlocked.Exchange(ref usingResource, 0);
                return true;
            }
            else
            {
                Console.WriteLine("   {0} was denied the lock", Thread.CurrentThread.Name);
                return false;
            }
        }
    }
}
