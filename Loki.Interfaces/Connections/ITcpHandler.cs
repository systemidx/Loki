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

        void Start();

        void Stop();

        Task<TcpClient> AcceptTcpClientAsync();
    }
}