using System;

namespace Loki.Common.Events
{
    public class PingEventArgs : EventArgs
    {
        public readonly byte[] Payload;

        public PingEventArgs(byte[] payload)
        {
            Payload = payload;
        }
    }
}
