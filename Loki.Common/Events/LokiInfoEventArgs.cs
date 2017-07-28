namespace Loki.Common.Events
{
    public class LokiInfoEventArgs : LokiEventArgs
    {
        /// <summary>
        /// The message
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="LokiInfoEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LokiInfoEventArgs(string message)
        {
            Message = message;
        }
    }
}
