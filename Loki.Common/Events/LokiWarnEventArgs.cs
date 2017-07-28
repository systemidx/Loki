namespace Loki.Common.Events
{
    public class LokiWarnEventArgs : LokiEventArgs
    {
        /// <summary>
        /// The message
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="LokiWarnEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LokiWarnEventArgs(string message)
        {
            Message = message;
        }
    }
}
