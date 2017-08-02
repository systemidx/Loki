using System;
using Loki.Common.Events;

namespace Loki.Interfaces.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        event EventHandler<LokiErrorEventArgs> OnError;

        /// <summary>
        /// Occurs when [on warn].
        /// </summary>
        event EventHandler<LokiWarnEventArgs> OnWarn;

        /// <summary>
        /// Occurs when [on debug].
        /// </summary>
        event EventHandler<LokiDebugEventArgs> OnDebug;

        /// <summary>
        /// Occurs when [on information].
        /// </summary>
        event EventHandler<LokiInfoEventArgs> OnInfo;

        /// <summary>
        /// Occurs when [on custom].
        /// </summary>
        event EventHandler<LokiCustomEventArgs> OnCustom;

        /// <summary>
        /// Errors the specified e.
        /// </summary>
        /// <param name="e">The exception.</param>
        void Error(Exception e);

        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Customs the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        void Custom(string eventType, string message);

        /// <summary>
        /// Customs the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        void Custom(Enum eventType, string message);

        /// <summary>
        /// Customs the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        void Custom(Type eventType, string message);
    }
}