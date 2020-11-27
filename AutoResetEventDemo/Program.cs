using System;
using System.Threading;

namespace AutoResetEventDemo
{
    class Program
    {
        private static AutoResetEvent event_1 = new AutoResetEvent(true);
        private static AutoResetEvent event_2 = new AutoResetEvent(false);

        static void Main()
        {
            Console.WriteLine("按Enter键创建三个线程并启动它们.\r\n" +
                            "这些线程等待在信号状态下创建的AutoResetEvent #1，\r\n" +
                            "所以第一个线程被释放.\r\n" +
                            "这使AutoResetEvent #1进入无信号状态。\r\n");
            Console.ReadLine();

            for (int i = 1; i < 4; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = "Thread_" + i;
                t.Start();
            }
            Thread.Sleep(250);

            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine("按Enter键释放一个线程。");
                Console.ReadLine();
                event_1.Set();
                Thread.Sleep(250);
            }

            Console.WriteLine("\r\n现在，所有线程都在等待AutoResetEvent＃2。");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("按Enter键释放一个线程。");
                Console.ReadLine();
                event_2.Set();
                Thread.Sleep(250);
            }

            Console.ReadLine();
        }

        static void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;

            Console.WriteLine("{0} 等待AutoResetEvent #1.", name);
            event_1.WaitOne();
            Console.WriteLine("{0} 从AutoResetEvent释放 #1.", name);

            Console.WriteLine("{0} 等待AutoResetEvent #2.", name);
            event_2.WaitOne();
            Console.WriteLine("{0} 从AutoResetEvent释放 #2.", name);

            Console.WriteLine("{0} 结束.", name);
        }
    }
}
