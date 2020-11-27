using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualResetEventSlimDemo
{
    class Program
    {
        static void Main()
        {
            MRES_SetWaitReset();
            MRES_SpinCountWaitHandle();
        }
        // 演示:
        //      ManualResetEventSlim construction
        //      ManualResetEventSlim.Wait()
        //      ManualResetEventSlim.Set()
        //      ManualResetEventSlim.Reset()
        //      ManualResetEventSlim.IsSet
        static void MRES_SetWaitReset()
        {
            ManualResetEventSlim mres1 = new ManualResetEventSlim(false); // 初始化为无信号
            ManualResetEventSlim mres2 = new ManualResetEventSlim(false); // 初始化为无信号
            ManualResetEventSlim mres3 = new ManualResetEventSlim(true);  // 初始化为有信号

            // 启动一个异步任务，操作mres3和mres2。
            var observer = Task.Factory.StartNew(() =>
            {
                mres1.Wait();
                Console.WriteLine("观察者看到信号mres1！");
                Console.WriteLine("观察者重置mres3 ...");
                mres3.Reset(); // 应该切换到无信号状态
                Console.WriteLine("观察者信号mres2");
                mres2.Set();
            });

            Console.WriteLine("线程: mres3.IsSet = {0} (应该为true)", mres3.IsSet);
            Console.WriteLine("主线程信号mres1");
            mres1.Set(); // 这将“启动”观察者任务
            mres2.Wait(); // 在观察者任务完成重新设置mres3之前，它不会返回
            Console.WriteLine("主线程看到信号mres2!");
            Console.WriteLine("线程: mres3.IsSet = {0} (应该为true)", mres3.IsSet);

            // 当你完成一个ManualResetEventSlim时，Dispose()是一种好的形式。
            observer.Wait(); // 确保这已经完全完成
            mres1.Dispose();
            mres2.Dispose();
            mres3.Dispose();
        }

        // 演示:
        //      ManualResetEventSlim construction w/ SpinCount
        //      ManualResetEventSlim.WaitHandle
        static void MRES_SpinCountWaitHandle()
        {
            // 构造一个SpinResetEventSlim，SpinCount为1000
            // Higher spincount => 锁定前，MRES会有更长的旋转等待时间。
            ManualResetEventSlim mres1 = new ManualResetEventSlim(false, 1000);
            ManualResetEventSlim mres2 = new ManualResetEventSlim(false, 1000);

            Task bgTask = Task.Factory.StartNew(() =>
            {
                // 稍等片刻
                Thread.Sleep(100);

                // 现在两个都发信号
                Console.WriteLine("任务同时发送两个MRES");
                mres1.Set();
                mres2.Set();
            });

            // MRES.WaitHandle的常见用法是将MRES用作WaitHandle.WaitAll / WaitAny的参与者。 
            // 请注意，访问MRES.WaitHandle将导致基础ManualResetEvent的无条件膨胀。
            WaitHandle.WaitAll(new WaitHandle[] { mres1.WaitHandle, mres2.WaitHandle });
            Console.WriteLine("WaitHandle.WaitAll(mres1.WaitHandle, mres2.WaitHandle) 已完成.");

            // 清理
            bgTask.Wait();
            mres1.Dispose();
            mres2.Dispose();
        }
    }
}
