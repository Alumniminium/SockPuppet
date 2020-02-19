using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SockPuppet
{
    public static class ProxyTester
    {
        private static Thread[] Threads;
        private static BlockingCollection<Proxy> Proxies = new BlockingCollection<Proxy>();

        public static void StartThreads(int threadCount)
        {
            Threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                Threads[i] = new Thread(WorkLoop);
                Threads[i].IsBackground = true;
                Threads[i].Start();
            }
        }

        public static void Enqueue(Proxy proxy) => Proxies.TryAdd(proxy);

        private static void WorkLoop()
        {
            foreach (var proxy in Proxies.GetConsumingEnumerable())
            {
                proxy.PerformTest(proxy);
                Console.WriteLine($"{(proxy.Alive ? "[ Up! ]" : "[Down!]")}{proxy}");
                Database.Locate(proxy);
                Database.AddToSortedList(proxy);
            }
        }
    }
}
