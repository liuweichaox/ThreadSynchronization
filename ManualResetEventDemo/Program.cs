using System;
using System.Threading;

namespace ManualResetEventDemo
{
    public class Program
    {
        // mre用于手动阻止和释放线程。
        // 它是在无信号状态下创建的。
        private static ManualResetEvent mre = new ManualResetEvent(false);

        static void Main()
        {
            Console.WriteLine("\n启动3个命名的线程，在ManualResetEvent上阻塞。\n");

            for (int i = 0; i <= 2; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = "Thread_" + i;
                t.Start();
            }

            Thread.Sleep(500);
            Console.WriteLine("\n当三个线程全部启动后，" +
                "\n按Enter键调用Set()来释放所有线程。\n");
            Console.ReadLine();

            mre.Set();

            Thread.Sleep(500);
            Console.WriteLine("\n当ManualResetEvent发出信号时，" +
                "\n调用WaitOne()的线程不会阻塞。按Enter键来显示这个。\n");
            Console.ReadLine();

            for (int i = 3; i <= 4; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = "Thread_" + i;
                t.Start();
            }

            Thread.Sleep(500);
            Console.WriteLine("\n按Enter键调用Reset()，" +
                "\n这样当线程调用WaitOne()时就会再次阻塞。\n");
            Console.ReadLine();

            mre.Reset();

            // 启动一个等待ManualResetEvent的线程。
            Thread t5 = new Thread(ThreadProc);
            t5.Name = "Thread_5";
            t5.Start();

            Thread.Sleep(500);
            Console.WriteLine("\n按Enter键调用Set()，结束演示。");
            Console.ReadLine();

            mre.Set();

            Console.ReadLine();
        }

        private static void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;

            Console.WriteLine(name + " starts and calls mre.WaitOne()");

            mre.WaitOne();

            Console.WriteLine(name + " ends.");
        }
    }
}
