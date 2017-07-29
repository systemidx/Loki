using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Loki.Common.Enum.Frame;
using Loki.Common.Events;
using Loki.Interfaces.Connections;
using Loki.Interfaces.Data;
using Loki.Interfaces.Dependency;
using Loki.Interfaces.Frame;
using Loki.Interfaces.Security;
using Loki.Interfaces.Threading;
using Loki.Server.Data;
using Loki.Server.Dependency;
using Loki.Server.Exceptions;
using Loki.Server.Frame;
using Loki.Server.Helpers;
using Loki.Server.Security;
using Loki.Server.Threading;

namespace Loki.Server.Connections
{
    internal sealed class WebSocketConnection : IWebSocketConnection
    {
        #region Readonly Variables
        
        /// <summary>
        /// The client
        /// </summary>
        private readonly TcpClient _client;

        /// <summary>
        /// The dependency utility
        /// </summary>
        private readonly IDependencyUtility _dependencyUtility;

        /// <summary>
        /// The security container
        /// </summary>
        private readonly ISecurityContainer _securityContainer;

        /// <summary>
        /// The data handlers
        /// </summary>
        private readonly IRouteTable _routeTable;
        
        /// <summary>
        /// The frame reader
        /// </summary>
        private readonly IWebSocketFrameReader _frameReader;

        /// <summary>
        /// The frame writer
        /// </summary>
        private readonly IWebSocketFrameWriter _frameWriter;

        /// <summary>
        /// The thread helper
        /// </summary>
        private readonly IThreadHelper _threadHelper;

        #endregion

        #region Member Variables
        
        /// <summary>
        /// The is disposed flag
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// The listening thread
        /// </summary>
        private Thread _listeningThread;

        /// <summary>
        /// The multi frame op code
        /// </summary>
        private WebSocketOpCode _multiFrameOpCode;

        #endregion

        #region Properties
        
        /// <summary>
        /// The HTTP metadata
        /// </summary>
        public IHttpMetadata HttpMetadata { get; private set; }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientIdentifier { get; set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid UniqueIdentifier { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive => !_isDisposed && _client.Connected && _client.Client.Connected;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketConnection" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="dependencyUtility">The dependency utility.</param>
        public WebSocketConnection(TcpClient client, IDependencyUtility dependencyUtility)
        {
            _client = client;
            _dependencyUtility = dependencyUtility ?? new DependencyUtility();
            _securityContainer = _dependencyUtility.Resolve<ISecurityContainer>() ?? new SecurityContainer(null, SslProtocols.None, false, false, false);
            _frameReader = _dependencyUtility.Resolve<IWebSocketFrameReader>() ?? new WebSocketFrameReader();
            _frameWriter = _dependencyUtility.Resolve<IWebSocketFrameWriter>() ?? new WebSocketFrameWriter();
            _routeTable = _dependencyUtility.Resolve<IRouteTable>() ?? new RouteTable();
            _threadHelper = _dependencyUtility.Resolve<IThreadHelper>() ?? new ThreadHelper();
        }

        #endregion
        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
            _isDisposed = true;
        }

        /// <summary>
        /// Blocks the and receive.
        /// </summary>
        public void Listen()
        {
            if (_listeningThread == null || !_listeningThread.IsAlive)
                _listeningThread = _threadHelper.CreateAndRun(Receive);
        }

        /// <summary>
        /// Sends the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        public void SendText<T>(T obj)
        {
            if (!IsAlive)
                return;

            string serializedObject = JsonConvert.SerializeObject(obj);
            
            _frameWriter.WriteText(serializedObject);
        }

        /// <summary>
        /// Sends the binary.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void SendBinary(byte[] obj)
        {
            if (!IsAlive)
                return;

            if (obj == null)
                return;

            _frameWriter.Write(WebSocketOpCode.BinaryFrame, obj);
        }

        #region Private Methods
        
        /// <summary>
        /// Receives this instance.
        /// </summary>
        /// <exception cref="WebSocketClosedException"></exception>
        private void Receive()
        {
            HandleStream();
;
            if (IsAlive)
                HandleHandshake();

            IWebSocketDataHandler handler = _routeTable[HttpMetadata.Route];
            if (handler == null)
            {
                KillConnection("Invalid route");
                return;
            }

            while (IsAlive)
            {
                HandleFrame(handler);

                //todo: Handle incoming queued messages

                Thread.Sleep(20);
            }
            
            Dispose();
        }

        /// <summary>
        /// Handles the frame.
        /// </summary>
        /// <param name="handler">The handler.</param>
        private void HandleFrame(IWebSocketDataHandler handler)
        {
            IWebSocketFrame frame = _frameReader.Read(_client);

            if (frame == null || !frame.IsValid)
                return;

            if (frame.OpCode == WebSocketOpCode.ContinuationFrame)
            {
                switch (_multiFrameOpCode)
                {
                    case WebSocketOpCode.TextFrame:
                        string message = Encoding.UTF8.GetString(frame.DecodedPayload, 0, frame.DecodedPayload.Length);

                        handler.OnTextPart(this, new TextMultiFrameEventArgs(message, frame.IsEntirePayload));
                        return;

                    case WebSocketOpCode.BinaryFrame:
                        handler.OnBinaryPart(this, new BinaryMultiFrameEventArgs(frame.DecodedPayload, frame.IsEntirePayload));
                        return;
                }
            }
            else
            {
                switch (frame.OpCode)
                {
                    case WebSocketOpCode.ConnectionClose:
                        handler.OnClose(this, GetConnectionCloseEventArgsFromPayload(frame.DecodedPayload));

                        KillConnection(string.Empty);
                        return;

                    case WebSocketOpCode.Ping:
                        handler.OnPing(this, new PingEventArgs(frame.DecodedPayload));
                        return;

                    case WebSocketOpCode.Pong:
                        handler.OnPong(this, new PingEventArgs(frame.DecodedPayload));
                        return;

                    case WebSocketOpCode.TextFrame:
                        string message = Encoding.UTF8.GetString(frame.DecodedPayload, 0, frame.DecodedPayload.Length);

                        if (frame.IsEntirePayload)
                        {
                            handler.OnText(this, new TextFrameEventArgs(message));
                        }
                        else
                        {
                            _multiFrameOpCode = frame.OpCode;
                            handler.OnTextPart(this, new TextMultiFrameEventArgs(message, frame.IsEntirePayload));
                        }
                        return;

                    case WebSocketOpCode.BinaryFrame:
                        if (frame.IsEntirePayload)
                        {
                            handler.OnBinary(this, new BinaryFrameEventArgs(frame.DecodedPayload));
                        }
                        else
                        {
                            _multiFrameOpCode = frame.OpCode;
                            handler.OnBinaryPart(this, new BinaryMultiFrameEventArgs(frame.DecodedPayload, frame.IsEntirePayload));
                        }
                        return;
                }
            }
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns></returns>
        private async void HandleStream()
        {
            if (!_client.Connected)
                return;

            Stream stream = _client.GetStream();

            if (!_securityContainer.Enabled)
            {
                _frameReader.Stream = stream;
                _frameWriter.Stream = stream;
                return;
            }

            try
            {
                SslStream sslStream = new SslStream(stream, false);
                await sslStream.AuthenticateAsServerAsync(_securityContainer.Certificate,
                    _securityContainer.ClientCertificateRequired,
                    _securityContainer.EnabledProtocols,
                    _securityContainer.CertificateRevocationEnabled);

                _frameReader.Stream = sslStream;
                _frameWriter.Stream = sslStream;
            }
            catch (AuthenticationException)
            {
                KillConnection("Unauthorized");
            }
            catch (IOException)
            {
                KillConnection(null);
            }
        }

        /// <summary>
        /// Kills the connection.
        /// </summary>
        private void KillConnection(string message)
        {
            if (message != null)
                _frameWriter.Write(WebSocketOpCode.ConnectionClose, Encoding.UTF8.GetBytes(message));
            
            Dispose();
        }

        /// <summary>
        /// Handles the handshake.
        /// </summary>
        private void HandleHandshake()
        {
            const int WEB_SOCKET_VERSION = 13;
            const string WEB_SOCKET_KEY_HEADER = "Sec-WebSocket-Key";
            const string WEB_SOCKET_VERSION_HEADER = "Sec-WebSocket-Version";
            
            HttpMetadata = new HttpMetadata(_frameReader.Stream);
            
            if (string.IsNullOrEmpty(HttpMetadata.Route) || HttpMetadata.Headers.Count == 0)
            { 
                KillConnection("Failed to read from stream");
                return;
            }

            if (!HttpMetadata.Headers.ContainsKey(WEB_SOCKET_KEY_HEADER) ||
                !HttpMetadata.Headers.ContainsKey(WEB_SOCKET_VERSION_HEADER))
            {
                KillConnection("Failed to retrieve websocket headers");
                return;
            }

            int secWebSocketVersion = Convert.ToInt32(HttpMetadata.Headers[WEB_SOCKET_VERSION_HEADER]);
            if (secWebSocketVersion < WEB_SOCKET_VERSION)
            {
                KillConnection($"WebSocket Version {secWebSocketVersion} not supported. Must be {WEB_SOCKET_VERSION} or higher");
                return;
            }

            string secWebSocketKey = HttpMetadata.Headers[WEB_SOCKET_KEY_HEADER];
            string secWebSocketAccept = ComputeSocketAcceptString(secWebSocketKey);
            string response = ComputeResponseString(secWebSocketAccept);

            HttpHelper.SetHeader(response, _frameReader.Stream);

            IWebSocketDataHandler handler = _routeTable[HttpMetadata.Route];
            handler?.OnOpen(this, new ConnectionOpenedEventArgs(HttpMetadata.QueryStrings));
        }
        
        /// <summary>
        /// Computes the socket accept string.
        /// </summary>
        /// <param name="secWebSocketKey">The sec web socket key.</param>
        /// <returns></returns>
        private string ComputeSocketAcceptString(string secWebSocketKey)
        {
            const string WEB_SOCKET_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

            string concatenated = secWebSocketKey + WEB_SOCKET_GUID;
            byte[] concatenatedAsBytes = Encoding.UTF8.GetBytes(concatenated);
            byte[] sha1Hash = SHA1.Create().ComputeHash(concatenatedAsBytes);
            string secWebSocketAccept = Convert.ToBase64String(sha1Hash);
            return secWebSocketAccept;
        }

        /// <summary>
        /// Computes the response string.
        /// </summary>
        /// <param name="secWebSocketAccept">The sec web socket accept.</param>
        /// <returns></returns>
        private string ComputeResponseString(string secWebSocketAccept)
        {
            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendLine("HTTP/1.1 101 Switching Protocols");
            responseBuilder.AppendLine("Connection: Upgrade");
            responseBuilder.AppendLine("Upgrade: websocket");
            responseBuilder.AppendLine("Sec-WebSocket-Accept: " + secWebSocketAccept);

            return responseBuilder.ToString();
        }

        /// <summary>
        /// Gets the connection close event arguments from payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        private ConnectionClosedEventArgs GetConnectionCloseEventArgsFromPayload(byte[] payload)
        {
            if (payload.Length >= 2)
            {
                using (MemoryStream stream = new MemoryStream(payload))
                {
                    ushort code = BinaryReaderWriter.ReadUShortExactly(stream, false);

                    try
                    {
                        WebSocketCloseCode closeCode = (WebSocketCloseCode)code;

                        if (payload.Length > 2)
                        {
                            string reason = Encoding.UTF8.GetString(payload, 2, payload.Length - 2);
                            return new ConnectionClosedEventArgs(closeCode, reason);
                        }

                        return new ConnectionClosedEventArgs(closeCode, null);
                    }
                    catch (InvalidCastException)
                    {
                        return new ConnectionClosedEventArgs(WebSocketCloseCode.Normal, null);
                    }
                }
            }

            return new ConnectionClosedEventArgs(WebSocketCloseCode.Normal, null);
        }

        #endregion
    }
}
