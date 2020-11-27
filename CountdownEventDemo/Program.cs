using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CountdownEventDemo
{
    class Program
    {
        static async Task Main()
        {
            // 初始化一个队列和一个CountdownEvent
            ConcurrentQueue<int> queue = new ConcurrentQueue<int>(Enumerable.Range(0, 10000));
            CountdownEvent cde = new CountdownEvent(10000); //初始计数= 10000

            // 这是所有队列使用者的逻辑
            Action consumer = () =>
            {
                int local;
                // 对于从队列中消耗的每个元素，CDE计数递减一次
                while (queue.TryDequeue(out local)) cde.Signal();
            };

            // 现在用两个异步任务清空队列
            Task t1 = Task.Factory.StartNew(consumer);
            Task t2 = Task.Factory.StartNew(consumer);

            // 通过等待cde来等待队列清空
            cde.Wait(); // 当cde计数达到0时返回

            Console.WriteLine("Done emptying queue.  InitialCount={0}, CurrentCount={1}, IsSet={2}",
                cde.InitialCount, cde.CurrentCount, cde.IsSet);

            //正确的形式是等待任务完成，即使你认为他们的工作已经完成。
            await Task.WhenAll(t1, t2);

            //重置将导致CountdownEvent取消设置，并将InitialCount/CurrentCount重置为指定值
            cde.Reset(10);

            //但currentAddCount不会影响InitialCount
            cde.AddCount(2);

            Console.WriteLine("After Reset(10), AddCount(2): InitialCount={0}, CurrentCount={1}, IsSet={2}",
                cde.InitialCount, cde.CurrentCount, cde.IsSet);

            //现在尝试取消等待
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel(); //取消CancellationTokenSource
            try
            {
                cde.Wait(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("cde.Wait(preCanceledToken) threw OCE, as expected");
            }
            finally
            {
                cts.Dispose();
            }
            //完成后释放CountdownEvent是很好的。
            cde.Dispose();
        }
    }
    // 示例显示如下输出:
    //    Done emptying queue.  InitialCount=10000, CurrentCount=0, IsSet=True
    //    After Reset(10), AddCount(2): InitialCount=10, CurrentCount=12, IsSet=False
    //    如预期的那样，cde.Wait(preCanceledToken)抛出OCE

}
