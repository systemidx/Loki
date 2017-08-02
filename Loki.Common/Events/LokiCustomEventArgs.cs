using System;

namespace Loki.Common.Events
{
    public class LokiCustomEventArgs: EventArgs
    {
        public readonly string EventType;
        public readonly string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="LokiCustomEventArgs"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public LokiCustomEventArgs(string eventType, string message)
        {
            Message = message;
            EventType = eventType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LokiCustomEventArgs"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public LokiCustomEventArgs(System.Enum eventType, string message) : this(eventType.ToString(), message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LokiCustomEventArgs"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public LokiCustomEventArgs(Type eventType, string message) : this(eventType.Name, message)
        {
            
        }
    }
}
