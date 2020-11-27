using System;
using System.Threading;

namespace EventWaitHandleDemo
{
    class Program
    {
        // 用于展示差异的EventWaitHandle。
        // 在AutoReset和ManualReset同步事件之间。
        //
        private static EventWaitHandle ewh;

        // 一个计数器，以确保所有线程的启动和
        //在任何一个被释放之前被阻止。长号是用来显示
        // 使用64位的Interlocked方法。
        //
        private static long threadCount = 0;

        // 一个允许主线程阻塞的AutoReset事件。
        // 直到一个退出的线程减少了计数。
        //
        private static EventWaitHandle clearCount =
            new EventWaitHandle(false, EventResetMode.AutoReset);

        [MTAThread]
        public static void Main()
        {
            // 创建一个AutoReset EventWaitHandle。
            //
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset);

            // 创建并启动五个编号的线程。使用
            // 参数化的ThreadStart委托，所以线程的
            // 可以将数字作为参数传递给Start 
            //方法。
            for (int i = 0; i <= 4; i++)
            {
                Thread t = new Thread(
                    new ParameterizedThreadStart(ThreadProc)
                );
                t.Start(i);
            }

            // 等到所有的线程都已经启动并阻塞。
            // 当多个线程在32位上使用一个64位的值时，就会在32位的
            // 系统，必须通过Interlocked类访问该值以保证线程安全。            
            while (Interlocked.Read(ref threadCount) < 5)
            {
                Thread.Sleep(500);
            }

            // 每当用户按下ENTER键时，释放一个线程。
            // 直到所有线程都被释放。
            //
            while (Interlocked.Read(ref threadCount) > 0)
            {
                Console.WriteLine("Press ENTER to release a waiting thread.");
                Console.ReadLine();

                // SignalAndWait向EventWaitHandle发出信号，
                // 在重置前正好释放一个线程，
                // 因为它是用AutoReset模式创建的。
                // 然后，SignalAndWait在clearCount上阻塞，
                // 以允许被信号的线程在再次循环之前减少计数。
                WaitHandle.SignalAndWait(ewh, clearCount);
            }
            Console.WriteLine();

            // 创建一个ManualReset EventWaitHandle。
            //
            ewh = new EventWaitHandle(false, EventResetMode.ManualReset);

            // 再创建并启动五个编号线程。
            //
            for (int i = 0; i <= 4; i++)
            {
                Thread t = new Thread(
                    new ParameterizedThreadStart(ThreadProc)
                );
                t.Start(i);
            }

            // 等到所有的线程都已经开始并被阻止。
            //
            while (Interlocked.Read(ref threadCount) < 5)
            {
                Thread.Sleep(500);
            }

            // 因为EventWaitHandle是用ManualReset模式创建的，
            // 所以发出信号释放所有等待的线程。
            Console.WriteLine("Press ENTER to release the waiting threads.");
            Console.ReadLine();
            ewh.Set();
        }

        public static void ThreadProc(object data)
        {
            int index = (int)data;

            Console.WriteLine("Thread {0} blocks.", data);
            //增加阻塞线程的数量。
            Interlocked.Increment(ref threadCount);

            // 等待事件的发生。
            ewh.WaitOne();

            Console.WriteLine("Thread {0} exits.", data);
            //减少阻塞线程的数量。
            Interlocked.Decrement(ref threadCount);

            // 发出信号ewh后，
            // 主线程在clearCount上阻塞，
            // 直到被信号的线程减少了计数。现在发出信号。
            clearCount.Set();
        }
    }
}
