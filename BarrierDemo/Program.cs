using System;
using System.Threading;
using System.Threading.Tasks;

namespace BarrierDemo
{
    class Program
    {
        //演示：
        //具有后阶段作用的屏障建造器
        //      Barrier.add参与者()
        //      障碍物移除参与者()
        //      障碍物、信号灯和等待（），包括抛出的BarrierPostPhaseException
        static void Main()
        {
            int count = 0;

            //设一道屏障，由三名参与者组成
            //提供将打印出某些信息的后阶段操作
            //第三次通过时，它将抛出一个异常
            Barrier barrier = new Barrier(3, (b) =>
            {
                Console.WriteLine("Post-Phase action: count={0}, phase={1}", count, b.CurrentPhaseNumber);
                if (b.CurrentPhaseNumber == 2) throw new Exception("D'oh!");
            });

            // Nope -- 改变我的主意了。 让它成为五个参与者。
            barrier.AddParticipants(2);

            // Nope -- 让我们确定四个参与者。
            barrier.RemoveParticipant();

            // 这是所有参与者运行的逻辑
            Action action = () =>
            {
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // 在后期操作中，count应该是4,phase应该是0
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // 在后期操作中，count应该是8,phase应该是1

                // 第三次，SignalAndWait()将抛出一个异常，所有参与者都将看到它
                Interlocked.Increment(ref count);
                try
                {
                    barrier.SignalAndWait();
                }
                catch (BarrierPostPhaseException bppe)
                {
                    Console.WriteLine("Caught BarrierPostPhaseException: {0}", bppe.Message);
                }

                // 第四次应该很顺利
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // 在后期操作中，count应该是16，而phase应该是3
            };

            // 现在启动4个并行的动作作为4个参与者
            Parallel.Invoke(action, action, action, action);

            //这（5个参与者）将导致异常：
            // Parallel.Invoke（action，action，action，action，action）;
            //System.InvalidOperationException：使用屏障的线程数
            //超出了注册参与者的总数。

            // 使用完屏障后，最好将其处理掉。
            barrier.Dispose();
        }
    }
}
