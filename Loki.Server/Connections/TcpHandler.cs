using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Loki.Interfaces.Connections;

namespace Loki.Server.Connections
{
    public class TcpHandler : ITcpHandler
    {
        /// <summary>
        /// The listener
        /// </summary>
        private readonly WrappedTcpListener _listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpHandler"/> class.
        /// </summary>
        /// <param name="localEp">The local ep.</param>
        public TcpHandler(IPEndPoint localEp)
        {
            _listener = new WrappedTcpListener(localEp);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpHandler"/> class.
        /// </summary>
        /// <param name="localaddr">An <see cref="T:System.Net.IPAddress" /> that represents the local IP address.</param>
        /// <param name="port">The port on which to listen for incoming connection attempts.</param>
        public TcpHandler(IPAddress localaddr, int port) : this(new IPEndPoint(localaddr, port))
        {
        }

        /// <summary>
        /// Gets a value that indicates whether <see cref="T:System.Net.Sockets.TcpListener" /> is actively listening for client connections.
        /// </summary>
        public bool IsAlive => _listener.Active;

        /// <summary>
        /// Accepts the TCP client asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<TcpClient> AcceptTcpClientAsync()
        {
            return await _listener.AcceptTcpClientAsync();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            _listener.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
        }
    }
}
