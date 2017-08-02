using System.Net.Sockets;
using System.Threading.Tasks;

namespace Loki.Interfaces.Connections
{
    public interface ITcpHandler
    {
        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        bool IsAlive { get; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();

        /// <summary>
        /// Accepts the TCP client asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<TcpClient> AcceptTcpClientAsync();
    }
}