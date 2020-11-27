using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpinWaitDemo
{
    class Program
    {
        // 演示:
        //      SpinWait construction
        //      SpinWait.SpinOnce()
        //      SpinWait.NextSpinWillYield
        //      SpinWait.Count
        static void Main()
        {
            bool someBoolean = false;
            int numYields = 0;

            // 第一个任务:自旋，直到someBoolean被设置为true
            Task t1 = Task.Factory.StartNew(() =>
            {
                SpinWait sw = new SpinWait();
                while (!someBoolean)
                {
                    //如果调用sw.SpinOnce()将导致生成处理器而不是简单地旋转，
                    //则NextSpinWillYield属性将返回true。
                    if (sw.NextSpinWillYield) numYields++;
                    sw.SpinOnce();
                }

                // 从.NET Framework 4开始：在进行一些初始旋转之后，SpinWait.SpinOnce（）将每次产生.
                Console.WriteLine("SpinWait called {0} times, yielded {1} times", sw.Count, numYields);
            });

            //第二个任务:等待100ms，然后设置someBoolean为true
            Task t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                someBoolean = true;
            });

            // 等待任务完成
            Task.WaitAll(t1, t2);
        }
    }
}
