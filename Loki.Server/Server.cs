using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Threading;
using Loki.Interfaces;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Data;
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
        private readonly int _listenerCount;

        /// <summary>
        /// The security container
        /// </summary>
        private readonly ISecurityContainer _securityContainer;

        /// <summary>
        /// The route table
        /// </summary>
        private readonly IRouteTable _routeTable;

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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="routeTable">The route table.</param>
        /// <param name="securityContainer">The security container.</param>
        /// <param name="listenerCount">The amount of threads to use for listening for new clients.</param>
        public Server(string id, string host, int port, IRouteTable routeTable = null, ISecurityContainer securityContainer = null, int listenerCount = 1)
        {
            Id = id;
            Host = host;
            Port = port;

            _securityContainer = securityContainer ?? new SecurityContainer(null, SslProtocols.None, false, false, false);
            _routeTable = routeTable ?? BuildRouteTable();

            _clientListener = new WrappedTcpListener(IPAddress.Parse(Host), Port);
            _connectionManager = new WebSocketConnectionManager(_routeTable, _securityContainer);
            _listenerCount = listenerCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="routeTable">The route table.</param>
        /// <param name="securityContainer">The security container.</param>
        public Server(string id, IPAddress host, int port, IRouteTable routeTable = null, ISecurityContainer securityContainer = null) 
            : this(id, host.ToString(), port, routeTable, securityContainer)
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
            catch (SocketException)
            {
                throw;
            }

            IsRunning = true;
         
            for (int i = 0; i < _listenerCount; ++i)
                ThreadHelper.CreateAndRun(Listen);

            ThreadHelper.CreateAndRun(EmitDiagnostics);

            HandleDeadConnections();
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public virtual void Stop()
        {
            IsRunning = false;
        }
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IsRunning = false;
            _clientListener.Stop();
        }

        /// <summary>
        /// Listens this instance.
        /// </summary>
        private async void Listen()
        {
            while (IsRunning)
            {
                if (_clientListener.Pending())
                {
                    TcpClient client = await _clientListener.AcceptTcpClientAsync();
                    _connectionManager.RegisterConnection(client);
                }

                Thread.Sleep(10);
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
        /// Emits the diagnostics.
        /// </summary>
        private void EmitDiagnostics()
        {
            while (IsRunning)
            {
                Console.Clear();
                Console.WriteLine($"Host: {this.Host}:{this.Port}");
                Console.WriteLine($"Total Connections: {_connectionManager.TotalConnections}");
                //Console.WriteLine($"Queued Connections: {_queues.Sum(x => x.Count)}");
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Gets the route table.
        /// </summary>
        private IRouteTable BuildRouteTable()
        {
            IRouteTable routeTable = new RouteTable();
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            IEnumerable<Type> types = entryAssembly.GetTypes().Where(x => x.GetTypeInfo().GetCustomAttributes().Any(y => y.GetType() == typeof(ConnectionRouteAttribute)));

            foreach (Type type in types)
            {
                IEnumerable<ConnectionRouteAttribute> attributes = type.GetTypeInfo().GetCustomAttributes<ConnectionRouteAttribute>();
                if (attributes == null)
                    continue;

                foreach (ConnectionRouteAttribute attribute in attributes)
                {
                    IWebSocketDataHandler handler = Activator.CreateInstance(type) as IWebSocketDataHandler;
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