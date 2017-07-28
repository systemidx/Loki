namespace Loki.Common.Events
{
    public class LokiDebugEventArgs : LokiEventArgs
    {
        public readonly string Message;

        public LokiDebugEventArgs(string message)
        {
            Message = message;
        }
    }
}
