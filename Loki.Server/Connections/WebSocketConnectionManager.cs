using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Dependency;
using Loki.Server.Dependency;

namespace Loki.Server.Connections
{
    public class WebSocketConnectionManager : IWebSocketConnectionManager
    {
        #region Readonly Variables

        /// <summary>
        /// The client map
        /// </summary>
        private readonly ConcurrentDictionary<IWebSocketConnection, object> _clientMap = new ConcurrentDictionary<IWebSocketConnection, object>();

        /// <summary>
        /// The dependency utility
        /// </summary>
        private readonly IDependencyUtility _dependencyUtility;
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the total connections.
        /// </summary>
        /// <value>
        /// The total connections.
        /// </value>
        public int TotalConnections => _clientMap.Count;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketConnectionManager" /> class.
        /// </summary>
        /// <param name="dependencyUtility">The dependency utility.</param>
        public WebSocketConnectionManager(IDependencyUtility dependencyUtility)
        {
            _dependencyUtility = dependencyUtility ?? new DependencyUtility();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void RegisterConnection(TcpClient connection)
        {
            if (connection == null)
                return;

            IWebSocketConnection socket = new WebSocketConnection(connection, _dependencyUtility);
            socket.Listen();

            _clientMap[socket] = new object();
        }

        /// <summary>
        /// Unregisters the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void UnregisterConnection(IWebSocketConnection connection)
        {
            const int MAX_CYCLES = 5;

            int i = 0;

            while (!_clientMap.TryRemove(connection, out _))
            {
                ++i;
                if (i >= MAX_CYCLES)
                    return;
            }
        }

        /// <summary>
        /// Gets the specified set of IWebSocketConnections with the same clientIdentifier.
        /// </summary>
        /// <param name="clientIdentifier">The clientIdentifier.</param>
        /// <returns></returns>
        public IWebSocketConnection[] GetConnectionsByClientIdentifier(string clientIdentifier)
        {
            return _clientMap.Where(x => x.Key.ClientIdentifier == clientIdentifier).Select(x => x.Key).ToArray();
        }

        /// <summary>
        /// Broadcasts the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Broadcast(string message)
        {
            foreach (var connection in _clientMap)
                if (connection.Key.IsAlive)
                    connection.Key.SendText(message);
        }

        /// <summary>
        /// Broadcasts the specified client identifier.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="message">The message.</param>
        public void Broadcast(string clientIdentifier, string message)
        {
            foreach (var connection in _clientMap.Where(x => x.Key.ClientIdentifier == clientIdentifier))
            {
                lock(connection.Value)
                { 
                    if (connection.Key.IsAlive)
                        connection.Key.SendText(message);
                }
            }
        }

        /// <summary>
        /// Broadcasts the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public void Broadcast(byte[] bytes)
        {
            foreach (var connection in _clientMap)
                if (connection.Key.IsAlive)
                    connection.Key.SendBinary(bytes);
        }

        /// <summary>
        /// Broadcasts the specified client identifier.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="bytes">The bytes.</param>
        public void Broadcast(string clientIdentifier, byte[] bytes)
        {
            foreach (var connection in _clientMap.Where(x => x.Key.ClientIdentifier == clientIdentifier))
            {
                lock (connection.Value)
                {
                    if (connection.Key.IsAlive)
                        connection.Key.SendBinary(bytes);
                }
            }
        }

        /// <summary>
        /// Removes the dead connections.
        /// </summary>
        public void RemoveDeadConnections()
        {
            foreach (var connection in _clientMap)
                if (!connection.Key.IsAlive)
                    UnregisterConnection(connection.Key);
        }

        
        #endregion
    }
}