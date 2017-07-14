using System;

namespace Loki.Server.Exceptions
{
    public class WebsocketException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsocketException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WebsocketException(string message) : base(message)
        {
            
        }
    }
}
