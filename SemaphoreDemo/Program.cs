using System;
using System.Threading;

namespace SemaphoreDemo
{
    class Program
    {
        //模拟有限资源池的信号量。
        private static Semaphore _pool;

        // 一个填充间隔，使输出更有序。
        private static int _padding;

        public static void Main()
        {
            //创建最多可以满足三个并发请求的信号量。
            //使用初始计数为零，
            //这样整个信号量计数最初由主程序线程拥有。
            //
            _pool = new Semaphore(0, 3);

            //创建并启动五个编号的线程。
            //
            for (int i = 1; i <= 5; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Worker));

                //启动线程，传递数字。
                //
                t.Start(i);
            }

            //等待半秒，
            //以允许所有线程在该信号量上启动和阻塞。
            //
            Thread.Sleep(500);

            //主线程开始时持有整个信号量计数。
            //调用Release(3)将信号量计数恢复到最大值，
            //并允许等待的线程进入信号量，
            //每次最多三个线程。
            //
            Console.WriteLine("主线程调用 Release(3).");
            _pool.Release(3);

            Console.WriteLine("主线程推出.");
        }

        private static void Worker(object num)
        {
            //每个工作线程都从请求信号量开始。
            Console.WriteLine("线程 {0} 开始 " +
                "等待信号量.", num);
            _pool.WaitOne();

            // 一个填充间隔，使输出更有序。
            int padding = Interlocked.Add(ref _padding, 100);

            Console.WriteLine("线程 {0} 进入信号量.", num);

            //线程的“工作”包括睡眠大约1秒。
            //每个线程“工作”的时间稍微长一些，
            //只是为了使输出更有序。
            //
            Thread.Sleep(1000 + padding);

            Console.WriteLine("线程 {0} 释放信号量", num);
            Console.WriteLine("线程 {0} 之前的信号量计数: {1}",
                num, _pool.Release());
        }
    }
}
