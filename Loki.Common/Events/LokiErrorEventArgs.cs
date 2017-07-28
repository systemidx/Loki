using System;

namespace Loki.Common.Events
{
    public class LokiErrorEventArgs : LokiEventArgs
    {
        public readonly Exception Exception;

        public LokiErrorEventArgs(Exception e)
        {
            Exception = e;
        }
    }
}
