using System;
using Loki.Common.Events;
using Loki.Interfaces.Logging;

namespace Loki.Server.Logging
{
    public class Logger : ILogger
    {
        public event EventHandler<LokiErrorEventArgs> OnError;
        public event EventHandler<LokiWarnEventArgs> OnWarn;
        public event EventHandler<LokiDebugEventArgs> OnDebug;
        public event EventHandler<LokiInfoEventArgs> OnInfo;

        /// <summary>
        /// Errors the specified e.
        /// </summary>
        /// <param name="e">The <see cref="LokiErrorEventArgs"/> instance containing the event data.</param>
        public void Error(Exception e)
        {
            OnError?.Invoke(this, new LokiErrorEventArgs(e));
        }

        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            OnWarn?.Invoke(this, new LokiWarnEventArgs(message));
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            OnDebug?.Invoke(this, new LokiDebugEventArgs(message));
        }

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            OnInfo?.Invoke(this, new LokiInfoEventArgs(message));
        }
    }
}
