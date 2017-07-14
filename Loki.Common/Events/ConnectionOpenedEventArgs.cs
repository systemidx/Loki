using System;
using System.Collections.Specialized;

namespace Loki.Common.Events
{
    public class ConnectionOpenedEventArgs : EventArgs
    {
        public readonly NameValueCollection Querystrings;

        public ConnectionOpenedEventArgs(NameValueCollection querystrings)
        {
            Querystrings = querystrings ?? new NameValueCollection();
        }
    }
}
