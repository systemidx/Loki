using System;
using System.Threading;

namespace Loki.Interfaces.Threading
{
    public interface IThreadHelper
    {
        /// <summary>
        /// Creates the thread and runs.
        /// </summary>
        /// <param name="threadStart">The thread start.</param>
        /// <returns></returns>
        Thread CreateAndRun(ThreadStart threadStart);

        /// <summary>
        /// Creates a thread and fires the specified action with arguments.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="arg">The argument.</param>
        void Create(Action<object> action, object arg = null);

        /// <summary>
        /// Creates a thread and fires the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        void Create(Action action);
    }
}