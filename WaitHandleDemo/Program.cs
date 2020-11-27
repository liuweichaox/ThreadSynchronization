using System;
using System.Threading;

namespace WaitHandleDemo
{
    class Program
    {
        // 定义一个带有两个AutoResetEvent WaitHandles的数组.
        static WaitHandle[] waitHandles = new WaitHandle[]
        {
        new AutoResetEvent(false),
        new AutoResetEvent(false)
        };

        // 定义用于测试的随机数生成器.
        static Random r = new Random();

        static void Main()
        {
            //在两个不同的线程上排列两个任务;
            //等待，直到所有的任务完成。
            DateTime dt = DateTime.Now;
            Console.WriteLine("主线程正在等待这两个任务的完成.");
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoTask), waitHandles[0]);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoTask), waitHandles[1]);
            WaitHandle.WaitAll(waitHandles);
            //下面显示的时间应该与最长的任务匹配.
            Console.WriteLine("两个任务都完成了 (等待时间={0})",
                (DateTime.Now - dt).TotalMilliseconds);

            //在两个不同的线程上排列两个任务;
            //等待任务完成。
            dt = DateTime.Now;
            Console.WriteLine();
            Console.WriteLine("主线程正在等待任一任务的完成.");
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoTask), waitHandles[0]);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoTask), waitHandles[1]);
            int index = WaitHandle.WaitAny(waitHandles);
            // 下面显示的时间应该匹配最短的任务.
            Console.WriteLine("任务 {0} 最先完成 (等待时间={1}).",
                index + 1, (DateTime.Now - dt).TotalMilliseconds);
        }

        static void DoTask(Object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            int time = 1000 * r.Next(2, 10);
            Console.WriteLine("执行任务的时间为 {0} 毫秒.", time);
            Thread.Sleep(time);
            are.Set();
        }
    }
}
