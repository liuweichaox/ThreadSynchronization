using System;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreSlimDemo
{
    public class Program
    {
        private static SemaphoreSlim semaphore;
        // 一个填充间隔，使输出更有序。
        private static int padding;

        public static void Main()
        {
            //创建信号量。
            semaphore = new SemaphoreSlim(0, 3);
            Console.WriteLine("{0} tasks can enter the semaphore.",
                              semaphore.CurrentCount);
            Task[] tasks = new Task[5];

            // 创建并启动5个编号的任务。
            for (int i = 0; i <= 4; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    // 每个任务都从请求信号量开始。
                    Console.WriteLine("任务 {0} 开始并等待信号量.",
                                      Task.CurrentId);

                    int semaphoreCount;
                    semaphore.Wait();
                    try
                    {
                        Interlocked.Add(ref padding, 100);

                        Console.WriteLine("任务 {0} 进入信号量.", Task.CurrentId);

                        // T任务只睡1+秒。
                        Thread.Sleep(1000 + padding);
                    }
                    finally
                    {
                        semaphoreCount = semaphore.Release();
                    }
                    Console.WriteLine("任务 {0} 释放信号量;以前的数: {1}.",
                                      Task.CurrentId, semaphoreCount);
                });
            }

            // 等待半秒，允许所有任务开始和停止.
            Thread.Sleep(500);

            // 将信号量计数恢复到最大值.
            Console.Write("主线程调用 Release(3) --> ");
            semaphore.Release(3);
            Console.WriteLine("{0} 任务可以进入这个信号量.",
                              semaphore.CurrentCount);
            // 主线程等待任务完成.
            Task.WaitAll(tasks);

            Console.WriteLine("主线程退出.");
        }
    }
}
