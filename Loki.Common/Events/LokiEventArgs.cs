using System;

namespace Loki.Common.Events
{
    public abstract class LokiEventArgs: EventArgs
    {
        public readonly DateTime EventTimeStamp;

        protected LokiEventArgs()
        {
            EventTimeStamp = DateTime.UtcNow;
        }
    }
}
