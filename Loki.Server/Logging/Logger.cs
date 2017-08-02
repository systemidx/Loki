using System;
using Loki.Common.Events;
using Loki.Interfaces.Logging;

namespace Loki.Server.Logging
{
    public class Logger : ILogger
    {
        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        public event EventHandler<LokiErrorEventArgs> OnError;

        /// <summary>
        /// Occurs when [on warn].
        /// </summary>
        public event EventHandler<LokiWarnEventArgs> OnWarn;

        /// <summary>
        /// Occurs when [on debug].
        /// </summary>
        public event EventHandler<LokiDebugEventArgs> OnDebug;

        /// <summary>
        /// Occurs when [on information].
        /// </summary>
        public event EventHandler<LokiInfoEventArgs> OnInfo;

        /// <summary>
        /// Occurs when [on custom].
        /// </summary>
        public event EventHandler<LokiCustomEventArgs> OnCustom;

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

        /// <summary>
        /// Customs the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public void Custom(string eventType, string message)
        {
            OnCustom?.Invoke(this, new LokiCustomEventArgs(eventType, message));
        }

        /// <summary>
        /// Customs the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public void Custom(Enum eventType, string message)
        {
            OnCustom?.Invoke(this, new LokiCustomEventArgs(eventType, message));
        }

        /// <summary>
        /// Customs the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public void Custom(Type eventType, string message)
        {
            OnCustom?.Invoke(this, new LokiCustomEventArgs(eventType, message));
        }
    }
}
