using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReaderWriterLockSlimDemo
{
    class Program
    {
        public static void Main()
        {
            var sc = new SynchronizedCache();
            var tasks = new List<Task>();
            int itemsWritten = 0;

            // 执行写入
            tasks.Add(Task.Run(() => {
                String[] vegetables = { "broccoli", "cauliflower",
                                                          "carrot", "sorrel", "baby turnip",
                                                          "beet", "brussel sprout",
                                                          "cabbage", "plantain",
                                                          "spinach", "grape leaves",
                                                          "lime leaves", "corn",
                                                          "radish", "cucumber",
                                                          "raddichio", "lima beans" };
                for (int ctr = 1; ctr <= vegetables.Length; ctr++)
                    sc.Add(ctr, vegetables[ctr - 1]);

                itemsWritten = vegetables.Length;
                Console.WriteLine("任务 {0} 写入 {1} items\n",
                                  Task.CurrentId, itemsWritten);
            }));
            // 执行两个读取器，一个从第一个读取到最后一个，另一个从最后一个读取到第一个。
            for (int ctr = 0; ctr <= 1; ctr++)
            {
                bool desc = Convert.ToBoolean(ctr);
                tasks.Add(Task.Run(() => {
                    int start, last, step;
                    int items;
                    do
                    {
                        String output = String.Empty;
                        items = sc.Count;
                        if (!desc)
                        {
                            start = 1;
                            step = 1;
                            last = items;
                        }
                        else
                        {
                            start = items;
                            step = -1;
                            last = 1;
                        }

                        for (int index = start; desc ? index >= last : index <= last; index += step)
                            output += String.Format("[{0}] ", sc.Read(index));

                        Console.WriteLine("任务 {0} 读取 {1} items: {2}\n",
                                          Task.CurrentId, items, output);
                    } while (items < itemsWritten | itemsWritten == 0);
                }));
            }
            // 执行读取/更新任务。
            tasks.Add(Task.Run(() => {
                Thread.Sleep(100);
                for (int ctr = 1; ctr <= sc.Count; ctr++)
                {
                    String value = sc.Read(ctr);
                    if (value == "cucumber")
                        if (sc.AddOrUpdate(ctr, "green bean") != SynchronizedCache.AddOrUpdateStatus.Unchanged)
                            Console.WriteLine("改变 'cucumber' 为 'green bean'");
                }
            }));

            // 等待这三个任务全部完成。
            Task.WaitAll(tasks.ToArray());

            // 显示缓存的最终内容。
            Console.WriteLine();
            Console.WriteLine("Values in synchronized cache: ");
            for (int ctr = 1; ctr <= sc.Count; ctr++)
                Console.WriteLine("   {0}: {1}", ctr, sc.Read(ctr));
        }
    }
}

