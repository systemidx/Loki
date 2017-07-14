using System;

namespace Loki.Server.Exceptions
{
    public class HttpHeaderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeaderException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpHeaderException(string message) : base(message)
        {
            
        }
    }
}
