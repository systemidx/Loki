using System.Net;
using System.Net.Sockets;

namespace Loki.Server.Connections
{
    public class WrappedTcpListener : TcpListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedTcpListener"/> class.
        /// </summary>
        /// <param name="localaddr">The localaddr.</param>
        /// <param name="port">The port.</param>
        public WrappedTcpListener(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedTcpListener"/> class.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public WrappedTcpListener(IPEndPoint localEp) : base(localEp)
        {
        }

        public new bool Active => base.Active;
    }
}
