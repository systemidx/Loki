using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Threading;
using Loki.Common.Events;
using Loki.Interfaces;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Data;
using Loki.Interfaces.Logging;
using Loki.Interfaces.Security;
using Loki.Server.Attributes;
using Loki.Server.Connections;
using Loki.Server.Data;
using Loki.Server.Helpers;
using Loki.Server.Security;

namespace Loki.Server
{
    public class Server : IServer
    {
        #region Readonly Variables

        /// <summary>
        /// The client listener
        /// </summary>
        private readonly WrappedTcpListener _clientListener;

        /// <summary>
        /// The connection manager
        /// </summary>
        private readonly IWebSocketConnectionManager _connectionManager;
        
        /// <summary>
        /// The amount of threads listening and processing clients
        /// </summary>
        private readonly int _listenerThreads;

        /// <summary>
        /// The client thread multiplier
        /// </summary>
        private readonly int _clientThreadMultiplier;

        /// <summary>
        /// The security container
        /// </summary>
        private readonly ISecurityContainer _securityContainer;

        /// <summary>
        /// The route table
        /// </summary>
        private readonly IRouteTable _routeTable;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The handler threads
        /// </summary>
        private readonly List<Thread> _handlerThreads = new List<Thread>();

        /// <summary>
        /// The incoming client queue
        /// </summary>
        private readonly ConcurrentQueue<TcpClient> _incomingClientQueue = new ConcurrentQueue<TcpClient>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; protected set; }

        /// <summary>
        /// The host address.
        /// </summary>
        public string Host { get; protected set; }

        /// <summary>
        /// The port.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Determines whether the server is running.
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// The flag which represents usage of Nagle's Algorithm
        /// </summary>
        public bool NoDelay { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="routeTable">The route table.</param>
        /// <param name="securityContainer">The security container.</param>
        /// <param name="listenerThreads">The amount of threads to use for listening for new clients.</param>
        /// <param name="clientThreadMultiplier">The multiplicand which represents the amount of handler threads to create in order to empty the listener queue.</param>
        public Server(string id, string host, int port, ILogger logger = null, IRouteTable routeTable = null, ISecurityContainer securityContainer = null, int listenerThreads = 1, int clientThreadMultiplier = 3)
        {
            Id = id;
            Host = host;
            Port = port;

            _logger = logger;
            _listenerThreads = listenerThreads;
            _clientThreadMultiplier = clientThreadMultiplier;

            _securityContainer = securityContainer ?? new SecurityContainer(null, SslProtocols.None, false, false, false);
            _routeTable = routeTable ?? BuildRouteTable();

            _clientListener = new WrappedTcpListener(IPAddress.Parse(Host), Port);
            _connectionManager = new WebSocketConnectionManager(_routeTable, _securityContainer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="routeTable">The route table.</param>
        /// <param name="securityContainer">The security container.</param>
        /// <param name="listenerThreads">The amount of threads to use for listening for new clients.</param>
        /// <param name="clientThreadMultiplier">The multiplicand which represents the amount of handler threads to create in order to empty the listener queue.</param>
        public Server(string id, IPAddress host, int port, ILogger logger = null, IRouteTable routeTable = null, ISecurityContainer securityContainer = null, int listenerThreads = 1, int clientThreadMultiplier = 3)
            : this(id, host.ToString(), port, logger, routeTable, securityContainer, listenerThreads, clientThreadMultiplier)
        {
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void RunAndBlock()
        {
            try
            {
                _clientListener.Start();
            }
            catch (SocketException e)
            {
                _logger.Error(e);

                throw;
            }

            IsRunning = true;

            _logger.Debug($"Starting {_listenerThreads} listener threads");
            for(int i = 0; i < _listenerThreads; ++i)
                _handlerThreads.Add(ThreadHelper.CreateAndRun(Listen));

            _logger.Debug($"Starting {_listenerThreads * _clientThreadMultiplier} client handler threads");
            for (int i = 0; i < _listenerThreads * _clientThreadMultiplier; ++i)
                _handlerThreads.Add(ThreadHelper.CreateAndRun(HandleIncomingClients));

            _logger.Debug("Starting dead connection handler thread");
            _handlerThreads.Add(ThreadHelper.CreateAndRun(HandleDeadConnections));
            
            while (IsRunning)
                Thread.Sleep(100);
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            _logger.Info("Server stopping");

            IsRunning = false;

            while (_handlerThreads.Any(x => x.IsAlive))
            {
                _logger.Info(".");
                Thread.Sleep(50);
            }

            _clientListener.Stop();

            Thread.Sleep(5000);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Listens this instance.
        /// </summary>
        private async void Listen()
        {
            while (IsRunning)
            {
                try
                {
                    TcpClient client = await _clientListener.AcceptTcpClientAsync();
                    client.Client.NoDelay = NoDelay;

                    _incomingClientQueue.Enqueue(client);
                }
                catch (SocketException)
                {
                }
            }
        }

        /// <summary>
        /// Handles the incoming clients.
        /// </summary>
        private void HandleIncomingClients()
        {
            while (IsRunning)
            {
                _incomingClientQueue.TryDequeue(out TcpClient client);

                if (client == null)
                {
                    Thread.Sleep(50);
                    continue;
                }

                _connectionManager.RegisterConnection(client);
            }
        }

        /// <summary>
        /// Handles the dead connections.
        /// </summary>
        private void HandleDeadConnections()
        {
            while (IsRunning)
            {
                _connectionManager.RemoveDeadConnections();

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Gets the route table.
        /// </summary>
        private IRouteTable BuildRouteTable()
        {
            IRouteTable routeTable = new RouteTable();
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            IEnumerable<Type> types = entryAssembly.GetTypes()
                .Where(x => x.GetTypeInfo()
                    .GetCustomAttributes()
                    .Any(y => y.GetType() == typeof(ConnectionRouteAttribute)));

            foreach (Type type in types)
            {
                IEnumerable<ConnectionRouteAttribute> attributes =
                    type.GetTypeInfo().GetCustomAttributes<ConnectionRouteAttribute>();
                if (attributes == null)
                    continue;

                foreach (ConnectionRouteAttribute attribute in attributes)
                {
                    IWebSocketDataHandler handler = Activator.CreateInstance(type, _logger) as IWebSocketDataHandler;
                    if (handler == null)
                        continue;

                    routeTable[attribute.Route] = handler;
                }
            }

            return routeTable;
        }

        #endregion
    }
}