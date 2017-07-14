using System;
using Loki.Common.Enum.Frame;

namespace Loki.Common.Events
{
    public class ConnectionClosedEventArgs : EventArgs
    {
        /// <summary>
        /// The code
        /// </summary>
        public readonly WebSocketCloseCode Code;

        /// <summary>
        /// The reason
        /// </summary>
        public readonly string Reason;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionClosedEventArgs"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="reason">The reason.</param>
        public ConnectionClosedEventArgs(WebSocketCloseCode code, string reason)
        {
            Code = code;
            Reason = reason;
        }
    }
}