using System;
using System.Threading;

namespace Loki.Server.Helpers
{
    public static class ThreadHelper
    {
        public static Thread CreateAndRun(ThreadStart threadStart)
        {
            if (threadStart == null)
                throw new ArgumentNullException(nameof(threadStart));

            //Thread thread = new Thread(threadStart, 262144);
            Thread thread = new Thread(threadStart);
            thread.Start();

            return thread;
        }

        public static void Create(Action<object> action, object arg = null)
        {
            ThreadPool.QueueUserWorkItem(x => action?.Invoke(arg));
        }

        public static void Create(Action action)
        {
            ThreadPool.QueueUserWorkItem(x => action?.Invoke());
        }
    }
}
