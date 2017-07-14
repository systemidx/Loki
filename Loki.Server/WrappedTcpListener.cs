using System.Net;
using System.Net.Sockets;

namespace Loki.Server
{
    public class WrappedTcpListener : TcpListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedTcpListener"/> class.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public WrappedTcpListener(IPEndPoint localEp) : base(localEp)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedTcpListener"/> class.
        /// </summary>
        /// <param name="localaddr">An <see cref="T:System.Net.IPAddress" /> that represents the local IP address.</param>
        /// <param name="port">The port on which to listen for incoming connection attempts.</param>
        public WrappedTcpListener(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        /// <summary>
        /// Gets a value that indicates whether <see cref="T:System.Net.Sockets.TcpListener" /> is actively listening for client connections.
        /// </summary>
        public new bool Active => base.Active;
    }
}
