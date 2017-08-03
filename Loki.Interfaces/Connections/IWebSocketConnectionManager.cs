using System.Net.Sockets;

namespace Loki.Interfaces.Connections
{
    public interface IWebSocketConnectionManager
    {
        /// <summary>
        /// Gets the total connections.
        /// </summary>
        /// <value>
        /// The total connections.
        /// </value>
        int TotalConnections { get; }
        
        /// <summary>
        /// Registers the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        void RegisterConnection(TcpClient connection);

        /// <summary>
        /// Unregisters the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        void UnregisterConnection(IWebSocketConnection connection);

        /// <summary>
        /// Removes the dead connections.
        /// </summary>
        void RemoveDeadConnections();

        /// <summary>
        /// Gets the specified set of IWebSocketConnections with the same clientIdentifier.
        /// </summary>
        /// <param name="clientIdentifier">The clientIdentifier.</param>
        /// <returns></returns>
        IWebSocketConnection[] GetConnectionsByClientIdentifier(string clientIdentifier);

        /// <summary>
        /// Broadcasts the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Broadcast(string message);

        /// <summary>
        /// Broadcasts the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        void Broadcast(byte[] bytes);
    }
}