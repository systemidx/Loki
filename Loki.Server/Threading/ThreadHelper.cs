using System;
using System.Threading;
using Loki.Interfaces.Threading;

namespace Loki.Server.Threading
{
    public class ThreadHelper : IThreadHelper
    {
        /// <summary>
        /// Creates the thread and runs.
        /// </summary>
        /// <param name="threadStart">The thread start.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">threadStart</exception>
        public Thread CreateAndRun(ThreadStart threadStart)
        {
            if (threadStart == null)
                throw new ArgumentNullException(nameof(threadStart));

            Thread thread = new Thread(threadStart);
            thread.Start();

            return thread;
        }

        /// <summary>
        /// Creates a thread and fires the specified action with arguments.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="arg">The argument.</param>
        public void Create(Action<object> action, object arg = null)
        {
            ThreadPool.QueueUserWorkItem(x => action?.Invoke(arg));
        }

        /// <summary>
        /// Creates a thread and fires the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Create(Action action)
        {
            ThreadPool.QueueUserWorkItem(x => action?.Invoke());
        }
    }
}